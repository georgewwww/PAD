using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Domain.Entities;
using WebServer.Domain.Events;

namespace WebServer.Application.Abstractions
{
    public interface IActorRepository : IRepository<Actor>, IEventSynchronizer<Actor, ActorEventEntity>
    { }
}
