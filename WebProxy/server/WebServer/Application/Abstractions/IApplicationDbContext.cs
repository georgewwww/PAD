using MongoDB.Driver;
using WebServer.Domain.Entities;

namespace WebServer.Application.Abstractions
{
    public interface IApplicationDbContext
    {
        IConnectionDatabaseSettings DatabaseSettings { get; }
        IMongoDatabase Database { get; }
        IMongoCollection<Actor> Actors { get; }
        IMongoCollection<Movie> Movies { get; }
    }
}
