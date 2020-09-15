using System.Collections.Generic;

namespace Common
{
    public class Payload
    {
        public string Bank { get; set; }
        public Dictionary<Currency, double> Rates { get; }

        public Payload()
        {
            Rates = new Dictionary<Currency, double>();
        }
    }
}
