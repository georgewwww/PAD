using System;
using WebServer.Application.Abstractions.Domain;

namespace WebServer.Domain.Events
{
    public class MovieEventEntity : IEventEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PosterLink { get; set; }
        public string Genre { get; set; }
        public int PremYear { get; set; }
        public string Time { get; set; }
        public float Score { get; set; }
        public string Description { get; set; }
    }
}
