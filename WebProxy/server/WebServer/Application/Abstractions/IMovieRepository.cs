using WebServer.Domain.Entities;
using WebServer.Domain.Events;

namespace WebServer.Application.Abstractions
{
    public interface IMovieRepository : IRepository<Movie>, IEventSynchronizer<Movie, MovieEventEntity>
    { }
}
