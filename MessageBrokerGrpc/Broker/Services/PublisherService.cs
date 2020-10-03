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

        public override async Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext context)
        {
            try
            {
                var message = new Message(request.Bank, request.Content);

                await _messageRepository.Add(message, context.CancellationToken);

                Console.WriteLine($"Received: {request.Bank}, {request.Content}");

                return await Task.FromResult(new PublishReply
                {
                    IsSuccess = true
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Publishing gRPC message: {e.Message}");
                return await Task.FromResult(new PublishReply
                {
                    IsSuccess = false
                });
            }
        }
    }
}
