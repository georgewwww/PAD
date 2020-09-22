using Common;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Subscriber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Settings.SUBSCRIBER_ADDRESS)
                .Build();

            host.Start();
            await Subscribe(host);

            Console.ReadLine();
        }

        private static async Task Subscribe(IWebHost host)
        {
            var address = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            Console.WriteLine($"Subscriber listening at: {address}");

            Console.Write("Enter the bank you want to subscribe: ");
            var bankName = Console.ReadLine().ToLower();
            
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var channel = GrpcChannel.ForAddress(Settings.BROKER_ADDRESS, new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new GrpcAgent.Subscriber.SubscriberClient(channel);

            var request = new SubscribeRequest { Address = address, Bank = bankName };

            try
            {
                var reply = await client.SubscribeAsync(request);
                Console.WriteLine($"Subscribed reply: {reply.IsSuccess}");
            }
            catch (RpcException e)
            {
                Console.WriteLine($"Details: {e.Status.Detail}");
                Console.WriteLine($"Status code: {e.Status.StatusCode}");
                Console.WriteLine($"Status code int: {(int)e.Status.StatusCode}");
            }
        }
    }
}
