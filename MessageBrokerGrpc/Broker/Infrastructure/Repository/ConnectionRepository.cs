using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task Add(Connection connection, CancellationToken cancellationToken)
        {
            await _dbContext.Connections.InsertOneAsync(connection, cancellationToken: cancellationToken);
        }

        public async Task Remove(string address, CancellationToken cancellationToken)
        {
            var deleteFilter = Builders<Connection>.Filter.Eq(c => c.Address, address);
            await _dbContext.Connections.DeleteOneAsync(deleteFilter, cancellationToken: cancellationToken);
        }

        public async Task<IList<Connection>> GetConnectionsByBank(string bank, CancellationToken cancellationToken)
        {
            var filteredConnections = await _dbContext.Connections.FindAsync(c => c.Bank == bank, cancellationToken: cancellationToken);
            return filteredConnections.ToList();
        }
    }
}
