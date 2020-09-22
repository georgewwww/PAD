using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Broker.Infrastructure.Repository;
using Broker.Models;
using Common;
using Grpc.Core;
using GrpcAgent;
using Newtonsoft.Json;

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
            var rates = JsonConvert.DeserializeObject<Dictionary<Currency, double>>(request.Content);
            var message = new Message(request.Bank, rates);

            _messageRepository.Add(message);

            Console.WriteLine($"Received: {request.Bank}, {rates}");

            return Task.FromResult(new PublishReply
            {
                IsSuccess = true
            });
        }
    }
}
