﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Broker.Infrastructure.Repository;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broker.Services
{
    public class SenderWorker : IHostedService
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
            _timer = new Timer(async o =>
            {
                await DoSendWork(cancellationToken);
            }, null, 0, TimeToWait);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task DoSendWork(CancellationToken cancellationToken)
        {
            var isEmptyMessages = await _messageRepository.IsEmpty(cancellationToken);
            if (isEmptyMessages) return;

            var message = await _messageRepository.GetNext(cancellationToken);
            if (message != null)
            {
                var connections = await _connectionRepository.GetConnectionsByBank(message.Bank, cancellationToken);

                if (connections.Count == 0)
                {
                    await _messageRepository.Add(message, cancellationToken);
                }

                foreach (var connection in connections)
                {
                    var channel = GrpcChannel.ForAddress(connection.Address, new GrpcChannelOptions { HttpHandler = _httpHandler });
                    var client = new Notifier.NotifierClient(channel);
                    var request = new NotifyRequest { Bank = message.Bank, Content = message.Rates };

                    try
                    {
                        var reply = client.Notify(request);
                        Console.WriteLine($"Notified subscriber {connection.Address} with {message.Rates}. Response: {reply.IsSuccess}");
                    }
                    catch (RpcException e)
                    {
                        if (e.StatusCode == StatusCode.Internal)
                        {
                            await _connectionRepository.Remove(connection.Address, cancellationToken);
                        }

                        Console.WriteLine($"Details: {e.Status.Detail}");
                        Console.WriteLine($"Status code: {e.Status.StatusCode}");
                    }
                }
            }
        }
    }
}
