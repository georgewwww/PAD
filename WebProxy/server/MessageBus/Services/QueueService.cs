using Common.Models;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MessageBus.Services
{
    public class QueueService : IQueueService
    {
        private readonly IBus bus;
        private readonly IConfiguration configuration;

        public QueueService(IBus bus, IConfiguration configuration)
        {
            this.bus = bus;
            this.configuration = configuration;
        }

        public async Task Enqueue(ServerEvent serverEvent)
        {
            await Publish("server-listener", serverEvent);
        }

        public async Task Publish<T>(string queue, T message)
        {
            var uri = new Uri(string.Concat($"rabbitmq://{configuration.GetConnectionString("MessageBrokerHost")}/", queue));
            var endPoint = await bus.GetSendEndpoint(uri);
            await endPoint.Send(message);
        }
    }

    public interface IQueueService
    {
        Task Enqueue(ServerEvent serverEvent);
        Task Publish<T>(string queue, T message);
    }
}
