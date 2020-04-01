using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Azure.SignalR.Management;
using Microsoft.AspNetCore.SignalR.Client;

namespace AzureDemos.SignalR.Client
{
    public class SignalRPublisher
    {
        private readonly IServiceManager serviceManager;

        private ImmutableDictionary<string, IObservable<IServiceHubContext>> hubContexts =
            ImmutableDictionary<string, IObservable<IServiceHubContext>>.Empty;

        public SignalRPublisher(string connectionString) =>
            this.serviceManager = new ServiceManagerBuilder()
                .WithOptions(option =>
                {
                    option.ConnectionString = connectionString;
                    option.ServiceTransportType = ServiceTransportType.Transient;
                })
                .Build();

        private IObservable<IServiceHubContext> CreateContextObservable(string hubName) =>
            serviceManager
                .CreateHubContextAsync(hubName)
                .ToObservable();

        private IObservable<IServiceHubContext> GetContextObservable(string hubName) =>
            ImmutableInterlocked
                .GetOrAdd(
                    location: ref hubContexts,
                    key: hubName,
                    valueFactory: CreateContextObservable
                );

        public Task PublishAsync<T>(string hubName, string methodName, T message) =>
            GetContextObservable(hubName)
                .FirstAsync()
                .Select(
                    con => con.Clients.All
                        .SendCoreAsync(
                            method: methodName,
                            args: new object[] { message }
                        )
                        .ToObservable()
                )
                .Switch()
                .ToTask();
    }

}
