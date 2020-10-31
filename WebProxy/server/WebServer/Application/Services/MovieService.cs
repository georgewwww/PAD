using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Application.Services
{
    public class MovieService : AbstractService<Movie>, IMovieService
    {
        public MovieService(IMovieRepository repository) : base(repository)
        { }
    }
}
