using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace SignalRConsole
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await Client.StartAsync();

            Console.WriteLine("Listening. Press a key to quit");
            Console.ReadKey();
        }
    }
    public class Client
    {
        private const string message = "Hello!";
        private static readonly TaskCompletionSource<string> resp = new TaskCompletionSource<string>();

        public static async Task StartAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.Register(() => resp.TrySetCanceled());
            var connection = new HubConnectionBuilder().WithUrl("https://localhost:44361/contenthub")
                .AddMessagePackProtocol()
                .Build();
            connection.On<dynamic>("Items", _resp => resp.TrySetResult(_resp));
            //connection.On<dynamic>("Items", async _resp =>Console.WriteLine($"Somebody ordered an {await resp.TrySetResult(_resp)}"));
            await connection.StartAsync(cancellationToken);
            await connection.InvokeAsync<dynamic>("GetItems",  cancellationToken);
            Console.WriteLine(await resp.Task);
        }
    }
}
