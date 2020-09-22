using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAgent;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sender");

            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var channel = GrpcChannel.ForAddress(Settings.BROKER_ADDRESS, new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new Publisher.PublisherClient(channel);

            Console.Write("Enter the bank name: ");
            var bankName = Console.ReadLine().ToLower();
            var rates = new Dictionary<Currency, double>();

            while (true)
            {
                rates.Clear();
                Console.WriteLine("Enter the rates for each currency (0 - not present)");
                
                foreach (var suit in (Currency[])Enum.GetValues(typeof(Currency)))
                {
                    Console.Write($" # {suit}: ");
                    var rate = Convert.ToDouble(Console.ReadLine());
                    if (rate > 0)
                    {
                        rates.Add(suit, rate);
                    }
                }

                var ratesString = JsonConvert.SerializeObject(rates);

                var request = new PublishRequest
                {
                    Bank = bankName,
                    Content = ratesString
                };

                try
                {
                    var reply = await client.PublishMessageAsync(request);
                    Console.WriteLine($"Publish reply: {reply.IsSuccess}");
                }
                catch (RpcException e)
                {
                    Console.WriteLine($"Details: {e.Status.Detail}");
                    Console.WriteLine($"Status code: {e.Status.StatusCode}");
                    Console.WriteLine($"Status code int: {(int)e.Status.StatusCode}");
                }

                Console.ReadLine();
            }
        }
    }
}
