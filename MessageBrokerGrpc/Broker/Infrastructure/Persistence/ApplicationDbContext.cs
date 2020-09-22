using Broker.Models;
using MongoDB.Driver;

namespace Broker.Infrastructure.Persistence
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        public ApplicationDbContext(IConnectionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            Connections = database.GetCollection<Connection>(settings.ConnectionsCollectionName);
            Messages = database.GetCollection<Message>(settings.MessagesCollectionName);
        }

        public IMongoCollection<Connection> Connections { get; }
        public IMongoCollection<Message> Messages { get; }
    }
}
