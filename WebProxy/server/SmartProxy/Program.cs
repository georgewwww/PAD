using MessageBroker;
using MessageBroker.Models;
using SmartProxy.LoadDistribution;
using StackExchange.Redis;
using System;
using System.Net;

namespace SmartProxy
{
    class Program
    {
        private static MessageBus messageBus;
        private static HttpListener httpListener;
        private static LoadBalancer loadBalancer;
        private static ConnectionMultiplexer connectionMultiplexer;

        static void Main(string[] args)
        {
            messageBus = new MessageBus(Environment.GetEnvironmentVariable("MessageBrokerConnectionString"));
            httpListener = new HttpListener();
            loadBalancer = new LoadBalancer();
            connectionMultiplexer = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("RedisHost"));

            var loadBalancerListener = new LoadBalancerListener(httpListener, loadBalancer, connectionMultiplexer);
            messageBus.Subscribe<ServerEvent>("Server", serverEvent =>
            {
                Console.WriteLine($"Server up: {serverEvent.Url}");
                loadBalancer.Add(new Uri(serverEvent.Url));
            });
            loadBalancerListener.Listen();
        }
    }
}
