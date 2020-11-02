using System;

namespace WebServer.Domain.Events
{
    public class EntityUpdateEvent<T>
    {
        public Guid EmittedServerId { get; set; }
        public T Entity { get; set; }
    }
}
