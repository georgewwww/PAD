using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Abstractions
{
    public interface IMessageBrokerPersistance
    {
        void Add(ServerInfo serverInfo);
        void Remove(string id);
        IList<ServerInfo> Get();
        IList<ServerInfo> GetExcept(string id);
    }
}
