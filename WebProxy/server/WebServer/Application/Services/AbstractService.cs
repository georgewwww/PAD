using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;

namespace WebServer.Application.Services
{
    public class AbstractService<T> : IService<T> where T : class, IEntity
    {
        private readonly IRepository<T> repository;

        public AbstractService(IRepository<T> repository)
        {
            this.repository = repository;
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var entity = await repository.Get(id, cancellationToken);

            if (entity == null)
            {
                throw new Exception("Entity not found");
            }

            await repository.Delete(id, cancellationToken);
        }

        public async Task<T> Get(Guid id, CancellationToken cancellationToken)
        {
            return await repository.Get(id, cancellationToken);
        }

        public async Task<IList<T>> Get(CancellationToken cancellationToken)
        {
            return await repository.Get(cancellationToken);
        }

        public async Task<T> Insert(T entity, CancellationToken cancellationToken)
        {
            return await repository.Insert(entity, cancellationToken);
        }

        public async Task<T> Update(T entity, CancellationToken cancellationToken)
        {
            return await repository.Update(entity, cancellationToken);
        }
    }
}
