using Common;
using System;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Subscriber");

            string bankName;

            Console.Write("Enter the bank you want to subscribe: ");
            bankName = Console.ReadLine().ToLower();

            var subscriberSocket = new SubscriberSocket(bankName);

            subscriberSocket.Connect(Settings.BROKER_IP, Settings.BROKER_PORT);

            Console.ReadLine();
        }
    }
}
