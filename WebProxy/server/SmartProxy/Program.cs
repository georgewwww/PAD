using Common.Models;
using GreenPipes;
using MassTransit;
using SmartProxy.LoadDistribution;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SmartProxy
{
    class Program
    {
        private static HttpListener httpListener;
        private static LoadBalancer loadBalancer;
        private static ConnectionMultiplexer connectionMultiplexer;

        static async Task Main(string[] args)
        {
            loadBalancer = new LoadBalancer();
            httpListener = new HttpListener();
            connectionMultiplexer = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("RedisHost") + ",allowAdmin=true");

            var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host(Environment.GetEnvironmentVariable("MessageBrokerHost"));
                cfg.ReceiveEndpoint("server-listener", e =>
                {
                    e.PrefetchCount = 8;
                    e.UseMessageRetry(r => r.Interval(2, 100));
                    e.Handler<ServerEvent>(context =>
                    {
                        loadBalancer.Add(new Uri(context.Message.Url));
                        return Console.Out.WriteLineAsync($"Server up: {context.Message.Url}");
                    });
                });
            });

            await busControl.StartAsync();

            Console.WriteLine("Press enter to exit");

            var loadBalancerListener = new LoadBalancerListener(httpListener, loadBalancer, connectionMultiplexer);
            loadBalancerListener.Listen();

            try
            {
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
                loadBalancerListener.Stop();
            }
        }
    }
}
