using Broker.Models;
using MongoDB.Driver;

namespace Broker.Infrastructure.Persistence
{
    public interface IApplicationDbContext
    {

        IMongoCollection<Connection> Connections { get; }
        IMongoCollection<Message> Messages { get; }
    }
}
