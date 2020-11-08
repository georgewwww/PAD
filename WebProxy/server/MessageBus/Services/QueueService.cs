using MassTransit;
using Microsoft.Extensions.Configuration;
using System;

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

        public async void Publish<T>(string queue, T message)
        {
            var uri = new Uri(string.Concat($"rabbitmq://{configuration.GetConnectionString("MessageBrokerHost")}/", queue));
            var endPoint = await bus.GetSendEndpoint(uri);
            await endPoint.Send(message);
        }
    }

    public interface IQueueService
    {
        void Publish<T>(string queue, T message);
    }
}
