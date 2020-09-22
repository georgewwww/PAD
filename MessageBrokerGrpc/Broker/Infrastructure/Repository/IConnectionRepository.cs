using System.Collections.Generic;
using Broker.Models;

namespace Broker.Infrastructure.Repository
{
    public interface IConnectionRepository
    {
        void Add(Connection connection);
        void Remove(string address);
        IList<Connection> GetConnectionsByBank(string bank);
    }
}
