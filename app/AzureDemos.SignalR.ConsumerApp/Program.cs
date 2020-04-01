using System;
using System.Reflection;
using static System.Console;
using AzureDemos.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Reactive.Concurrency;

namespace AzureDemos.SignalR.ConsumerApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var connectionString = config.ResolveValue("SignalR:ConnectionString");

            const string HubName = "hub1";
            const string UserName = "user1";
            const string MethodName = "method1";

            using var _ = new SignalRReceiver(connectionString)
                .Receive<string>(HubName, UserName, MethodName)
                .ObserveOn(NewThreadScheduler.Default)
                .Subscribe(
                    msg => WriteLine(TimeStamped("Received: " + msg)),
                    err => Error.WriteLine("Error: " + err)
                );

            WriteLine("Start listening.");

            Console.ReadKey();
        }

        private static string TimeStamped(string message) =>
            $"[{DateTime.Now.ToString("hh:mm:ss")}] {message}";
    }
}
