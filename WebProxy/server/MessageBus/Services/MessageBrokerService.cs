using Common;
using Grpc.Core;
using MessageBus.Abstractions;
using MessageBus.Models;
using MessageBus.Services;
using System;
using System.Threading.Tasks;

namespace MessageBroker.Services
{
    public class MessageBrokerService : Common.MessageBroker.MessageBrokerBase
    {
        private readonly IMessageBrokerPersistance messageBrokerPersistance;
        private readonly QueueService queueService;

        public MessageBrokerService(
            IMessageBrokerPersistance messageBrokerPersistance,
            QueueService queueService)
        {
            this.messageBrokerPersistance = messageBrokerPersistance;
            this.queueService = queueService;
        }

        public async override Task<SubscribeResponseMessage> SubscribeService(SubscribeRequestMessage request, ServerCallContext context)
        {
            bool response;
            try
            {
                var serverInfo = new ServerInfo
                {
                    Id = request.ServerId,
                    Host = request.ServerHost
                };
                messageBrokerPersistance.Add(serverInfo);
                Console.WriteLine($">> New server added: {serverInfo.Id} and host {serverInfo.Host} <<");
                response = true;
            }
            catch (Exception)
            {
                response = false;
            }

            return await Task.FromResult(new SubscribeResponseMessage
            {
                Success = response
            });
        }

        public async override Task<ResponseMessage> PublishInsert(RequestMessage request, ServerCallContext context)
        {
            bool response;
            try
            {
                foreach (var server in messageBrokerPersistance.Get()) {
                    queueService.Publish(server.Id, request.Payload);
                }

                response = true;
            }
            catch(Exception e)
            {
                response = false;
            }

            return await Task.FromResult(new ResponseMessage
            {
                Success = response
            });
        }
    }
}
