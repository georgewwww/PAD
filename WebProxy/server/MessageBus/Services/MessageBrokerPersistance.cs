using MessageBus.Abstractions;
using MessageBus.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MessageBus.Services
{
    public class MessageBrokerPersistance : IMessageBrokerPersistance
    {
        private readonly ConcurrentQueue<ServerInfo> ServicesIdentifiers;

        public MessageBrokerPersistance()
        {
            ServicesIdentifiers = new ConcurrentQueue<ServerInfo>();
        }

        public void Add(ServerInfo serverInfo)
        {
            ServicesIdentifiers.Enqueue(serverInfo);
        }

        public IList<ServerInfo> Get()
        {
            return ServicesIdentifiers.ToList();
        }

        public IList<ServerInfo> GetExcept(string id)
        {
            return ServicesIdentifiers.Where(si => si.Id != id).ToList();
        }
    }
}
