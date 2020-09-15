using Common;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sender");

            var senderSocket = new SenderSocket();
            senderSocket.Connect(Settings.BROKER_IP, Settings.BROKER_PORT);

            if (!senderSocket.isConnected)
            {
                Console.WriteLine("Error: Could not connected to Broker");
                return;
            }


            Console.Write("Enter the bank name: ");
            var bankName = Console.ReadLine().ToLower();

            while (true)
            {
                var payload = new Payload
                {
                    Bank = bankName
                };

                Console.WriteLine("Enter the rates for each currency (0 - not present)");
                
                foreach (Currency suit in (Currency[])Enum.GetValues(typeof(Currency)))
                {
                    Console.Write($" # {suit}: ");
                    double rate = Convert.ToDouble(Console.ReadLine());
                    if (rate > 0)
                    {
                        payload.Rates.Add(suit, rate);
                    }
                }

                var payloadString = JsonConvert.SerializeObject(payload);
                byte[] data = Encoding.UTF8.GetBytes(payloadString);

                senderSocket.Send(data);
                Console.ReadLine();
            }
        }
    }
}
