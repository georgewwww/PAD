using MongoDB.Driver;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Infrastructure.Persistence
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        public ApplicationDbContext(IConnectionDatabaseSettings settings)
        {
            DatabaseSettings = settings;
            var client = new MongoClient(DatabaseSettings.ConnectionString);
            Database = client.GetDatabase(DatabaseSettings.DatabaseName);
            Actors = Database.GetCollection<Actor>(DatabaseSettings.ActorsCollectionName);
            Movies = Database.GetCollection<Movie>(DatabaseSettings.MoviesCollectionName);
        }

        public IMongoDatabase Database { get; }
        public IConnectionDatabaseSettings DatabaseSettings { get; }
        public IMongoCollection<Actor> Actors { get; }
        public IMongoCollection<Movie> Movies { get; }
    }
}
