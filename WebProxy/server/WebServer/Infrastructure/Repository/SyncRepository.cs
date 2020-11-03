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

        public async void OnInsertEvent(EntityInsertEvent<TEvent> @event)
        {
            if (@event.EmittedServerId == serverDescriptor.Id)
            {
                Console.WriteLine("Skipping insert because emitter server id matches current");
            } else
            {
                var entity = CreateModel(@event.Entity);
                Console.WriteLine("Inserting entity to database: " + entity.Id);

                await Insert(entity, CancellationToken.None, false);
            }
        }

        public async void OnUpdateEvent(EntityUpdateEvent<TEvent> @event)
        {
            if (@event.EmittedServerId == serverDescriptor.Id)
            {
                Console.WriteLine("Skipping update because emitter server id matches current");
            } else
            {
                var eventEntity = @event.Entity;
                var entity = await Get(eventEntity.Id, new CancellationTokenSource().Token);
                Console.WriteLine("Updating entity from database: " + entity.Id);
                UpdateEntity(eventEntity, entity, false);
                await Update(entity, CancellationToken.None, false);
            }
        }

        public async void OnDeleteEvent(EntityDeleteEvent @event)
        {
            if (@event.EmittedServerId == serverDescriptor.Id)
            {
                Console.WriteLine("Skipping delete because emitter server id matches current");
            } else
            {
                Console.WriteLine("Deleting entity from database: " + @event.Id);
                await Delete(@event.Id, CancellationToken.None, false);
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
