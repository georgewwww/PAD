using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Domain.Entities;

namespace WebServer.Application.Abstractions
{
    public interface IActorRepository : IRepository<Actor>
    { }
}
