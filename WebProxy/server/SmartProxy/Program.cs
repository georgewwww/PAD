using SmartProxy.LoadDistribution;
using StackExchange.Redis;
using System;
using System.Net;

namespace SmartProxy
{
    class Program
    {
        private static HttpListener httpListener;
        private static LoadBalancer loadBalancer;
        private static ConnectionMultiplexer connectionMultiplexer;

        static void Main(string[] args)
        {
            // todo MessageBroker

            httpListener = new HttpListener();
            loadBalancer = new LoadBalancer();
            connectionMultiplexer = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("RedisHost"));

            var loadBalancerListener = new LoadBalancerListener(httpListener, loadBalancer, connectionMultiplexer);
            loadBalancerListener.Listen();
        }
    }
}
