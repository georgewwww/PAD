using System;
using System.Threading.Tasks;
using Broker.Infrastructure.Repository;
using Broker.Models;
using Grpc.Core;
using GrpcAgent;

namespace Broker.Services
{
    public class PublisherService : Publisher.PublisherBase
    {
        private readonly IMessageRepository _messageRepository;

        public PublisherService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public override Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext context)
        {
            var message = new Message(request.Bank, request.Content);

            _messageRepository.Add(message);

            Console.WriteLine($"Received: {request.Bank}, {request.Content}");

            return Task.FromResult(new PublishReply
            {
                IsSuccess = true
            });
        }
    }
}
