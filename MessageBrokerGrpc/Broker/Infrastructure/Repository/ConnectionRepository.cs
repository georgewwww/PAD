using System.Collections.Generic;
using System.Linq;
using Broker.Models;

namespace Broker.Infrastructure.Repository
{
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly List<Connection> _connections;
        private readonly object _locker;

        public ConnectionRepository()
        {
            _connections = new List<Connection>();
            _locker = new object();
        }

        public void Add(Connection connection)
        {
            lock (_locker)
            {
                _connections.Add(connection);
            }
        }

        public void Remove(string address)
        {
            lock (_locker)
            {
                _connections.RemoveAll(x => x.Address == address);
            }
        }

        public IList<Connection> GetConnectionsByBank(string bank)
        {
            lock (_locker)
            {

                var filteredConnections = _connections.Where(x => x.Bank == bank).ToList();
                return filteredConnections;
            }
        }
    }
}
