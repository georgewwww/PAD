using System.Collections.Generic;
using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Broker.Models
{
    public class Message
    {
        public Message(string bankName, string rates)
        {
            this.Bank = bankName;
            this.Rates = rates;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string Bank { get; set; }
        [BsonElement]
        public string Rates { get; set; }
    }
}
