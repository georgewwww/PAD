using System;
using MongoDB.Bson.Serialization.Attributes;
using WebServer.Application.Abstractions.Domain;

namespace WebServer.Domain.Entities
{
    public class Movie : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public string PosterLink { get; set; }
        [BsonElement]
        public string Genre { get; set; }
        [BsonElement]
        public int PremYear { get; set; }
        [BsonElement]
        public string Time { get; set; }
        [BsonElement]
        public float Score { get; set; }
        [BsonElement]
        public string Description { get; set; }

        public Movie()
        {
            Id = Guid.Empty;
            Name = string.Empty;
            PosterLink = string.Empty;
            Genre = string.Empty;
            PremYear = 0;
            Time = string.Empty;
            Score = 0f;
            Description = string.Empty;
        }
    }
}
