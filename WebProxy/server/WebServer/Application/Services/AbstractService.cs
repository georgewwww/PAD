using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;
using WebServer.Application.Abstractions.Domain;
using WebServer.Domain;

namespace WebServer.Application.Services
{
    public class AbstractService<T> : IService<T>
        where T : class, IEntity
    {
        private readonly IRepository<T> repository;

        public AbstractService(IRepository<T> repository)
        {
            this.repository = repository;
        }

        public async Task<IActionResponse> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await repository.Get(id, cancellationToken);

                if (entity == null)
                {
                    throw new Exception("Entity not found");
                }

                await repository.Delete(id, cancellationToken);

                return await Task.FromResult(new ActionResponse
                {
                    StatusCode = 200,
                    Message = string.Empty
                });
            }
            catch (Exception e)
            {
                return await Task.FromResult(new ActionResponse
                {
                    StatusCode = 500,
                    Message = e.Message
                });
            }
        }

        public async Task<T> Get(Guid id, CancellationToken cancellationToken)
        {
            return await repository.Get(id, cancellationToken);
        }

        public async Task<IList<T>> Get(CancellationToken cancellationToken)
        {
            return await repository.Get(cancellationToken);
        }

        public async Task<IActionResponse> Insert(T entity, CancellationToken cancellationToken)
        {
            try
            {
                await repository.Insert(entity, cancellationToken);

                return await Task.FromResult(new ActionResponse
                {
                    StatusCode = 200,
                    Message = string.Empty
                });
            }
            catch(Exception e)
            {
                return await Task.FromResult(new ActionResponse
                {
                    StatusCode = 500,
                    Message = e.Message
                });
            }
        }

        public async Task<IActionResponse> Update(T entity, CancellationToken cancellationToken)
        {
            try
            {
                await repository.Update(entity, cancellationToken);

                return await Task.FromResult(new ActionResponse
                {
                    StatusCode = 200,
                    Message = string.Empty
                });
            }
            catch (Exception e)
            {
                return await Task.FromResult(new ActionResponse
                {
                    StatusCode = 500,
                    Message = e.Message
                });
            }
        }
    }
}
