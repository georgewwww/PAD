using System.Collections.Generic;
using Common;

namespace Broker.Models
{
    public class Message
    {
        public Message(string bankName, Dictionary<Currency, double> rates)
        {
            this.Bank = bankName;
            this.Rates = rates;
        }

        public string Bank { get; set; }
        public Dictionary<Currency, double> Rates { get; set; }
    }
}
