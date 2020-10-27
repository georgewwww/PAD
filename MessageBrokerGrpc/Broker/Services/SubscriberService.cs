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

        public override async Task<SubscribeReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine($"New client subscribed: {request.Address} {request.Bank}");

                var connection = new Connection(request.Address, request.Bank);
                await _connectionRepository.Add(connection, context.CancellationToken);

                return await Task.FromResult(new SubscribeReply
                {
                    IsSuccess = true
                });
            }
            catch(Exception e)
            {
                Console.WriteLine($"Publishing gRPC message: {e.Message}");
                return await Task.FromResult(new SubscribeReply
                {
                    IsSuccess = false
                });
            }
        }
    }
}
