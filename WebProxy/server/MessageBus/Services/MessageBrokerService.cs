using Common;
using Common.Models;
using Grpc.Core;
using MessageBus.Abstractions;
using MessageBus.Models;
using MessageBus.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MessageBroker.Services
{
    public class MessageBrokerService : Common.MessageBroker.MessageBrokerBase
    {
        private readonly IMessageBrokerPersistance messageBrokerPersistance;
        private readonly IQueueService queueService;

        public MessageBrokerService(
            IMessageBrokerPersistance messageBrokerPersistance,
            IQueueService queueService)
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

                var serverEvent = new ServerEvent
                {
                    Url = request.ServerHost
                };
                await queueService.Enqueue(serverEvent);

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

        public async override Task<ResponseMessage> PublishEntity(RequestMessage request, ServerCallContext context)
        {
            bool response;
            string currentServerId = string.Empty;
            try
            {
                foreach (var server in messageBrokerPersistance.GetExcept(request.EmittedServerId)) {
                    currentServerId = server.Id;

                    var model = new Request
                    {
                        EmittedServerId = request.EmittedServerId,
                        Descriptive = request.Descriptive,
                        Payload = request.Payload
                    };

                    await queueService.Publish(server.Id, model);
                }

                response = true;
            }
            catch(Exception)
            {
                messageBrokerPersistance.Remove(currentServerId);
                response = false;
            }

            return await Task.FromResult(new ResponseMessage
            {
                Success = response
            });
        }
    }
}
