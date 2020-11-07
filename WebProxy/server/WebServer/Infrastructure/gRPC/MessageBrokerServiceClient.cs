using Common;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Application;

namespace WebServer.Infrastructure.gRPC
{
    public class MessageBrokerServiceClient : IMessageBrokerServiceClient
    {
        private readonly Common.MessageBroker.MessageBrokerClient client;
        private readonly ServerDescriptor serverDescriptor;

        public MessageBrokerServiceClient(ServerDescriptor serverDescriptor)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5000");
            client = new Common.MessageBroker.MessageBrokerClient(channel);
            this.serverDescriptor = serverDescriptor;
        }

        public async Task Subscribe(string id, string hostName)
        {
            var request = new SubscribeRequestMessage
            {
                ServerId = id,
                ServerHost = hostName
            };

            await client.SubscribeServiceAsync(request);
        }

        public async Task Publish(string id, string descriptive, string payload)
        {
            var request = new RequestMessage
            {
                EmittedServerId = serverDescriptor.Id.ToString(),
                Descriptive = descriptive,
                Payload = payload
            };

            await client.PublishInsertAsync(request);
        }
    }
}
