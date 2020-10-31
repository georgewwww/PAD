using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Application.Services
{
    public class ActorService : AbstractService<Actor>, IActorService
    {
        public ActorService(IActorRepository repository) : base(repository)
        { }
    }
}
