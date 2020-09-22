namespace Broker.Models
{
    public class Connection
    {
        public Connection(string address, string bankName)
        {
            this.Address = address;
            this.Bank = bankName;
        }

        public string Address { get; set; }
        public string Bank { get; set; }
    }
}
