using Common;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Subscriber
{
    public class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);
            var payload = JsonConvert.DeserializeObject<Payload>(payloadString);

            Console.WriteLine($"\nNew rates update at {payload.Bank}");
            foreach(var rate in payload.Rates)
            {
                Console.WriteLine($" # {rate.Key}: {rate.Value}");
            }
        }
    }
}
