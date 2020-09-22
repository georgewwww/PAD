using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Broker.Infrastructure.Repository;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Broker.Services
{
    public class SenderWorker : IHostedService, IDisposable
    {
        private Timer _timer;
        private const int TimeToWait = 2000;
        private readonly IMessageRepository _messageRepository;
        private readonly IConnectionRepository _connectionRepository;
        private readonly HttpClientHandler _httpHandler;

        public SenderWorker(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                _messageRepository = scope.ServiceProvider.GetService<IMessageRepository>();
                _connectionRepository = scope.ServiceProvider.GetService<IConnectionRepository>();
            }

            _httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoSendWork, null, 0, TimeToWait);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoSendWork(object state)
        {
            var isEmptyMessages = _messageRepository.IsEmpty();
            if (isEmptyMessages) return;

            var message = _messageRepository.GetNext();
            if (message != null)
            {
                var connections = _connectionRepository.GetConnectionsByBank(message.Bank);

                foreach (var connection in connections)
                {
                    var channel = GrpcChannel.ForAddress(connection.Address, new GrpcChannelOptions { HttpHandler = _httpHandler });
                    var client = new Notifier.NotifierClient(channel);
                    var ratesString = JsonConvert.SerializeObject(message.Rates);
                    var request = new NotifyRequest { Bank = message.Bank, Content =  ratesString };

                    try
                    {
                        var reply = client.Notify(request);
                        Console.WriteLine($"Notified subscriber {connection.Address} with {ratesString}. Response: {reply.IsSuccess}");
                    }
                    catch (RpcException e)
                    {
                        if (e.StatusCode == StatusCode.Internal)
                        {
                            _connectionRepository.Remove(connection.Address);
                        }

                        Console.WriteLine($"Details: {e.Status.Detail}");
                        Console.WriteLine($"Status code: {e.Status.StatusCode}");
                        Console.WriteLine($"Status code int: {(int)e.Status.StatusCode}");
                    }
                }
            }
        }

        public void Dispose() => _timer?.Dispose();
    }
}
