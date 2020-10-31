using MongoDB.Driver;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Infrastructure.Persistence
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        public ApplicationDbContext(IConnectionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            Actors = database.GetCollection<Actor>(settings.ActorsCollectionName);
            Movies = database.GetCollection<Movie>(settings.MoviesCollectionName);
        }

        public IMongoCollection<Actor> Actors { get; }
        public IMongoCollection<Movie> Movies { get; }
    }
}
