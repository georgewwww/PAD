using System;

namespace WebServer.Application
{
    public class ServerDescriptor
    {
        public string Url { get; set; }
        public Guid Id { get; }

        public ServerDescriptor()
        {
            Id = Guid.NewGuid();
        }

        public ServerDescriptor(Guid id)
        {
            Id = id;
        }
    }
}
