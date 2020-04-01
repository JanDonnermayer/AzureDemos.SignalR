using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.SignalR.Management;

namespace AzureDemos.SignalR.Client
{
    public class SignalRReceiver
    {
        private readonly IServiceManager serviceManager;

        private ImmutableDictionary<(string hubName, string userName), IObservable<HubConnection>> connections =
            ImmutableDictionary<(string hubName, string userName), IObservable<HubConnection>>.Empty;

        public SignalRReceiver(string connectionString) =>
            this.serviceManager = new ServiceManagerBuilder()
                .WithOptions(option =>
                {
                    option.ConnectionString = connectionString;
                    option.ServiceTransportType = ServiceTransportType.Transient;
                })
                .Build();

        private HubConnection GetConnection((string hubName, string userName) info)
        {
            var url = serviceManager
                .GetClientEndpoint(info.hubName)
                .TrimEnd('/') + "&user=" + info.userName;

            var accessToken = serviceManager
                .GenerateClientAccessToken(info.hubName);

            return new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(url, opt => opt.AccessTokenProvider = () => Task.FromResult(accessToken))
                .Build();
        }

        private static IObservable<HubConnection> Start(HubConnection connection) =>
            connection
                .StartAsync()
                .ToObservable()
                .Select(_ => connection);

        private static IObservable<T> ReceiveMessages<T>(HubConnection connection, string methodName) =>
            Observable.Create<T>(
                obs => connection.On<T>(
                    methodName: methodName,
                    handler: msg => obs.OnNext(msg)
                )
            );

        private IObservable<HubConnection> GetConnectionObservable(string hubName, string userName) =>
            ImmutableInterlocked
                .GetOrAdd(
                    location: ref connections,
                    key: (hubName, userName),
                    valueFactory: conInfo => Start(GetConnection(conInfo))
                );

        public IObservable<T> Receive<T>(string hubName, string userName, string methodName) =>
            GetConnectionObservable(hubName, userName)
                .Select(con => ReceiveMessages<T>(con, methodName))
                .Switch();
    }
}
