using System.Collections.Generic;
using Broker.Infrastructure.Persistence;
using Broker.Models;
using MongoDB.Driver;

namespace Broker.Infrastructure.Repository
{
    public class ConnectionRepository : IConnectionRepository
    {
        private IApplicationDbContext _dbContext;

        public ConnectionRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Connection connection)
        {
            _dbContext.Connections.InsertOne(connection);
        }

        public void Remove(string address)
        {
            var deleteFilter = Builders<Connection>.Filter.Eq(c => c.Address, address);
            _dbContext.Connections.DeleteOne(deleteFilter);
        }

        public IList<Connection> GetConnectionsByBank(string bank)
        {
            var filteredConnections = _dbContext.Connections.FindSync(c => c.Bank == bank);
            return filteredConnections.ToList();
        }
    }
}
