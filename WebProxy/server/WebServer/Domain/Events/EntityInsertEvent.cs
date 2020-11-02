using System;

namespace WebServer.Domain.Events
{
    public class EntityInsertEvent<T>
    {
        public Guid EmittedServerId { get; set; }
        public T Entity { get; set; }
    }
}
