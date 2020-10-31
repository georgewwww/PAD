using MongoDB.Driver;
using WebServer.Domain.Entities;

namespace WebServer.Application.Abstractions
{
    public interface IApplicationDbContext
    {
        IMongoCollection<Actor> Actors { get; }
        IMongoCollection<Movie> Movies { get; }
    }
}
