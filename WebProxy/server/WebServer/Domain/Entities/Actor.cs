using System;
using MongoDB.Bson.Serialization.Attributes;
using WebServer.Application.Abstractions.Domain;

namespace WebServer.Domain.Entities
{
    public class Actor : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement]
        public string FullName { get; set; }
        [BsonElement]
        public string ImageLink { get; set; }
        [BsonElement]
        public int BirthYear { get; set; }
        [BsonElement]
        public string BirthDate { get; set; }
        [BsonElement]
        public string Description { get; set; }

        public Actor()
        {
            Id = Guid.Empty;
            FullName = string.Empty;
            ImageLink = string.Empty;
            BirthYear = 0;
            BirthDate = string.Empty;
            Description = string.Empty;
        }
    }
}
