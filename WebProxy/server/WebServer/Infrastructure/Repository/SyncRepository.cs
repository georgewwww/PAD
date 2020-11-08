using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application;
using WebServer.Application.Abstractions;
using WebServer.Application.Abstractions.Domain;
using WebServer.Infrastructure.gRPC;

namespace WebServer.Infrastructure.Repository
{
    public abstract class SyncRepository<T> : IRepository<T>
        where T : class, IEntity
    {
        private readonly ServerDescriptor serverDescriptor;
        private readonly IMessageBrokerServiceClient messageBrokerServiceClient;

        public SyncRepository(
            ServerDescriptor serverDescriptor,
            IMessageBrokerServiceClient messageBrokerServiceClient)
        {
            this.serverDescriptor = serverDescriptor;
            this.messageBrokerServiceClient = messageBrokerServiceClient;
        }

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

            Console.WriteLine("Inserting entity to database: " + entity.Id);
            if (createEvent)
            {
                var entityPayload = JsonConvert.SerializeObject(entity);

                await messageBrokerServiceClient.Publish(
                    serverDescriptor.Id.ToString(),
                    string.Concat(nameof(Insert).ToLower(), "#", typeof(T).Name.ToLower()),
                    entityPayload);
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
                Console.WriteLine("Updating entity from database: " + entity.Id);

                if (createEvent)
                {
                    var entityPayload = JsonConvert.SerializeObject(entity);

                    await messageBrokerServiceClient.Publish(
                        serverDescriptor.Id.ToString(),
                        string.Concat(nameof(Update).ToLower(), "#", typeof(T).Name.ToLower()),
                        entityPayload);
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

            Console.WriteLine("Deleting entity from database: " + id);
            if (createEvent)
            {
                var entityPayload = JsonConvert.SerializeObject(CreateModelFromId(id));

                await messageBrokerServiceClient.Publish(
                    serverDescriptor.Id.ToString(),
                    string.Concat(nameof(Delete).ToLower(), "#", typeof(T).Name.ToLower()),
                    entityPayload);
            }
        }

        public abstract IMongoCollection<T> Collection { get; }

        public abstract FilterDefinition<T> DeleteFilter(Guid id);
        public abstract FilterDefinition<T> GetFilter(Guid id);
        public abstract FilterDefinition<T> UpdateGetFilter(Guid id);
        public abstract UpdateDefinition<T> UpdateFilter(T entity);

        public abstract T CreateModelFromId(Guid id);
    }
}
