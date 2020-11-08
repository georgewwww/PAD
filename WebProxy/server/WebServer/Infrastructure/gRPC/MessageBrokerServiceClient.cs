using Common;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using WebServer.Application;

namespace WebServer.Infrastructure.gRPC
{
    public class MessageBrokerServiceClient : IMessageBrokerServiceClient
    {
        private readonly MessageBroker.MessageBrokerClient client;
        private readonly ServerDescriptor serverDescriptor;

        public MessageBrokerServiceClient(
            ServerDescriptor serverDescriptor,
            IConfiguration configuration)
        {
            this.serverDescriptor = serverDescriptor;

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress($"http://{configuration.GetConnectionString("MessageBrokerGrpc")}:5000");
            client = new MessageBroker.MessageBrokerClient(channel);
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

            await client.PublishEntityAsync(request);
        }
    }
}
