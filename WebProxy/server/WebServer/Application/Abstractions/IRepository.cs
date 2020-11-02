using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions.Domain;

namespace WebServer.Application.Abstractions
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T> Get(Guid id, CancellationToken cancellationToken);
        Task<IList<T>> Get(CancellationToken cancellationToken);
        Task<T> Insert(T entity, CancellationToken cancellationToken, bool createEvent = true);
        Task<T> Update(T entity, CancellationToken cancellationToken, bool createEvent = true);
        Task Delete(Guid id, CancellationToken cancellationToken, bool createEvent = true);
    }
}
