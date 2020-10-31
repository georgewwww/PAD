using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;

namespace WebServer.Infrastructure.Repository
{
    public class SyncRepository<T> where T : class, IEntity
    {

    }
}
