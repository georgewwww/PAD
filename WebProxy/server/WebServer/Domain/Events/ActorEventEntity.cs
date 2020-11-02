using System;
using WebServer.Application.Abstractions.Domain;

namespace WebServer.Domain.Events
{
    public class ActorEventEntity : IEventEntity
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ImageLink { get; set; }
        public int BirthYear { get; set; }
        public string BirthDate { get; set; }
        public string Description { get; set; }
    }
}
