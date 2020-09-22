using System.Collections.Concurrent;
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

        public void Add(Message message)
        {
            _dbContext.Messages.InsertOne(message);
        }

        public Message GetNext()
        {
            var message = _dbContext.Messages.FindOneAndDelete(new BsonDocument());
            return message;
        }

        public bool IsEmpty()
        {
            return _dbContext.Messages.CountDocuments(new BsonDocument()) == 0;
        }
    }
}
