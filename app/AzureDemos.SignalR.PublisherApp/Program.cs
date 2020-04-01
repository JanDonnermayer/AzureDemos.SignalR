using System;
using System.Reflection;
using static System.Console;
using AzureDemos.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Reactive.Concurrency;

namespace AzureDemos.SignalR.PublisherApp
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

            const string HubName = "hub1";
            const string UserName = "user1";
            const string MethodName = "method1";

            while(true)
                await new SignalRPublisher(connectionString)
                    .PublishAsync(HubName, MethodName, PromptLine("Please enter message!"))
                    .ConfigureAwait(false);
        }

        private static string PromptLine(string message)
        {
            WriteLine(message);
            return ReadLine();
        }
    }
}
