using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Grpc.Core;
using GrpcAgent;
using Newtonsoft.Json;

namespace Subscriber.Services
{
    public class NotificationService : Notifier.NotifierBase
    {
        public override Task<NotifyReply> Notify(NotifyRequest request, ServerCallContext context)
        {
            var rates = JsonConvert.DeserializeObject<Dictionary<Currency, double>>(request.Content);

            Console.WriteLine($"\nNew rates update at {request.Bank}");
            foreach (var rate in rates)
            {
                Console.WriteLine($" # {rate.Key}: {rate.Value}");
            }

            return Task.FromResult(new NotifyReply { IsSuccess = true });
        }
    }
}
