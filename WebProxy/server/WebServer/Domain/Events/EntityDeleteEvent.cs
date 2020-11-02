using System;

namespace WebServer.Domain.Events
{
    public class EntityDeleteEvent
    {
        public Guid EmittedServerId { get; set; }
        public Guid Id { get; set; }
    }
}
