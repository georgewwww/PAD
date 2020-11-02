using MessageBroker;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application;
using WebServer.Application.Abstractions;
using WebServer.Application.Abstractions.Domain;
using WebServer.Domain.Events;

namespace WebServer.Infrastructure.Repository
{
    public abstract class SyncRepository<T, TEvent> : IRepository<T>, IEventSynchronizer<T, TEvent>
        where T : class, IEntity
        where TEvent : IEventEntity
    {
        private readonly MessageBus messageBroker;
        private readonly ServerDescriptor serverDescriptor;

        public SyncRepository(
            MessageBus messageBroker,
            ServerDescriptor serverDescriptor)
        {
            this.messageBroker = messageBroker;
            this.serverDescriptor = serverDescriptor;
        }

        public string InsertQueue => typeof(T).Name + "-insert";
        public string UpdateQueue => typeof(T).Name + "-update";
        public string DeleteQueue => typeof(T).Name + "-delete";

        public async Task<T> Get(Guid id, CancellationToken cancellationToken)
        {
            var filter = GetFilter(id);
            var entries = await Collection.FindAsync(filter, cancellationToken: cancellationToken);
            return entries.FirstOrDefault();
        }

        public async Task<IList<T>> Get(CancellationToken cancellationToken)
        {
            var entries = await Collection.FindAsync(a => true, cancellationToken: cancellationToken);
            return entries.ToList();
        }

        public async Task<T> Insert(T entity, CancellationToken cancellationToken, bool createEvent = true)
        {
            await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            if (createEvent)
            {
                messageBroker.Publish(InsertQueue, new EntityInsertEvent<TEvent>
                {
                    EmittedServerId = serverDescriptor.Id,
                    Entity = CreateEventModel(entity)
                });
            }
            return entity;
        }

        public async Task<T> Update(T entity, CancellationToken cancellationToken, bool createEvent = true)
        {
            var updateDefinition = UpdateFilter(entity);
            var getFilter = UpdateGetFilter(entity.Id);

            var result = await Collection.UpdateOneAsync(getFilter, updateDefinition, cancellationToken: cancellationToken);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                if (createEvent)
                {
                    messageBroker.Publish(UpdateQueue, new EntityUpdateEvent<TEvent>
                    {
                        EmittedServerId = serverDescriptor.Id,
                        Entity = CreateEventModel(entity)
                    });
                }
                return entity;
            }
            else
            {
                throw new NullReferenceException($"{typeof(T).Name} with id {entity.Id} not found");
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken, bool createEvent = true)
        {
            var filter = DeleteFilter(id);
            await Collection.DeleteOneAsync(filter, cancellationToken);
            if (createEvent)
            {
                messageBroker.Publish(DeleteQueue, new EntityDeleteEvent
                {
                    EmittedServerId = serverDescriptor.Id,
                    Id = id
                });
            }
        }

        public void OnInsertEvent(EntityInsertEvent<TEvent> @event)
        {
            if (@event.EmittedServerId == serverDescriptor.Id)
            {
                Console.WriteLine("Skipping because emitter server id matches current");
            } else
            {
                var entity = CreateModel(@event.Entity);

                Insert(entity, new CancellationTokenSource().Token).Wait();
            }
        }

        public void OnUpdateEvent(EntityUpdateEvent<TEvent> @event)
        {
            if (@event.EmittedServerId == serverDescriptor.Id)
            {
                Console.WriteLine("Skipping because emitter server id matches current");
            } else
            {
                var eventEntity = @event.Entity;
                var entity = Get(eventEntity.Id, new CancellationTokenSource().Token).Result;
                UpdateEntity(eventEntity, entity, false);
                Update(entity, new CancellationTokenSource().Token, false).Wait();
            }
        }

        public void OnDeleteEvent(EntityDeleteEvent @event)
        {
            if (@event.EmittedServerId == serverDescriptor.Id)
            {
                Console.WriteLine("Skipping because emitter server id matches current");
            } else
            {
                var entity = Get(@event.Id, new CancellationTokenSource().Token).Result;
                Delete(entity.Id, new CancellationTokenSource().Token).Wait();
            }
        }

        public abstract IMongoCollection<T> Collection { get; }

        public abstract FilterDefinition<T> DeleteFilter(Guid id);
        public abstract FilterDefinition<T> GetFilter(Guid id);
        public abstract FilterDefinition<T> UpdateGetFilter(Guid id);
        public abstract UpdateDefinition<T> UpdateFilter(T entity);

        public abstract T CreateModel(TEvent entityEvent);
        public abstract TEvent CreateEventModel(T entity);
        public abstract void UpdateEntity(TEvent @event, T entity, bool copyId);
    }
}
