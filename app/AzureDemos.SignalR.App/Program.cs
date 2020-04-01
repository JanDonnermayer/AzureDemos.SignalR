using System;
using System.Reflection;
using static System.Console;
using AzureDemos.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AzureDemos.SignalR.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.ResolveValue("SignalR:ConnectionString");

            const string MethodName = "method1";
            const string HubName = "hub1";
            const string UserName = "user1";

            using var _ = new SignalRReceiver(connectionString)
                .Receive<string>(HubName, UserName, MethodName)
                .Subscribe(WriteLine, err => Error.WriteLine(err));

            while (true)
            {
                await new SignalRPublisher(connectionString)
                    .PublishAsync(HubName, MethodName, PromptLine("Please enter message!"))
                    .ConfigureAwait(false);
            }
        }

        private static string PromptLine(string message)
        {
            WriteLine(message);
            return ReadLine();
        }
    }
}
