using Common;
using System.Collections.Generic;
using System.Linq;

namespace Broker
{
    static class ConnectionsStorage
    {
        private static readonly List<ConnectionInfo> _connections;
        private static readonly object _locker;

        static ConnectionsStorage()
        {
            _connections = new List<ConnectionInfo>();
            _locker = new object();
        }

        public static void Add(ConnectionInfo connection)
        {
            lock(_locker)
            {
                _connections.Add(connection);
            }
        }

        public static void Remove(string address)
        {
            lock (_locker)
            {
                _connections.RemoveAll(x => x.Address == address);
            }
        }

        public static List<ConnectionInfo> GetConnectionByBankName(string bankName)
        {
            List<ConnectionInfo> selectedConnections;

            lock (_locker)
            {
                selectedConnections = _connections.Where(x => x.BankName == bankName).ToList();
            }

            return selectedConnections;
        }
    }
}
