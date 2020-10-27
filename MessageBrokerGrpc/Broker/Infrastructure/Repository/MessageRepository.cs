using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Broker.Infrastructure.Persistence;
using Broker.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Broker.Infrastructure.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private IApplicationDbContext _dbContext { get; }

        public MessageRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Message message, CancellationToken cancellationToken)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Bank, message.Bank);

            await _dbContext.Messages.ReplaceOneAsync(filter, message, new ReplaceOptions { IsUpsert = true, },
                cancellationToken);
        }

        public async Task<Message> GetNext(CancellationToken cancellationToken)
        {
            var message = _dbContext.Messages.FindOneAndDeleteAsync(new BsonDocument(), cancellationToken: cancellationToken);
            return await message;
        }

        public async Task<bool> IsEmpty(CancellationToken cancellationToken)
        {
            var count = await _dbContext.Messages.CountDocumentsAsync(new BsonDocument(), cancellationToken: cancellationToken);
            return count == 0;
        }
    }
}
