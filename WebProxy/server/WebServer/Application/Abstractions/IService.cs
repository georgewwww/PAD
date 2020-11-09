using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions.Domain;

namespace WebServer.Application.Abstractions
{
    public interface IService<T>
        where T : class, IEntity
    {
        Task<T> Get(Guid id, CancellationToken cancellationToken);
        Task<IList<T>> Get(CancellationToken cancellationToken);
        Task<IActionResponse> Insert(T entity, CancellationToken cancellationToken);
        Task<IActionResponse> Update(T entity, CancellationToken cancellationToken);
        Task<IActionResponse> Delete(Guid id, CancellationToken cancellationToken);
    }
}
