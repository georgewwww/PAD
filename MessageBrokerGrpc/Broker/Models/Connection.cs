using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Broker.Models
{
    public class Connection
    {
        public Connection(string address, string bankName)
        {
            this.Address = address;
            this.Bank = bankName;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } 

        [BsonElement]
        public string Address { get; set; }
        [BsonElement]
        public string Bank { get; set; }
    }
}
