using System;
using System.Threading.Tasks;
using Broker.Infrastructure.Repository;
using Broker.Models;
using Grpc.Core;
using GrpcAgent;

namespace Broker.Services
{
    public class SubscriberService : Subscriber.SubscriberBase
    {
        private readonly IConnectionRepository _connectionRepository;

        public SubscriberService(IConnectionRepository connectionRepository)
        {
            _connectionRepository = connectionRepository;
        }

        public override Task<SubscribeReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            Console.WriteLine($"New client subscribed: {request.Address} {request.Bank}");

            var connection = new Connection(request.Address, request.Bank);
            _connectionRepository.Add(connection);

            return Task.FromResult(new SubscribeReply
            {
                IsSuccess = true
            });
        }
    }
}
