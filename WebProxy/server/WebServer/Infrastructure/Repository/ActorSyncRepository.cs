using MongoDB.Driver;
using System;
using WebServer.Application;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;
using WebServer.Infrastructure.gRPC;

namespace WebServer.Infrastructure.Repository
{
    public class ActorSyncRepository : SyncRepository<Actor>, IActorRepository
    {
        private readonly IApplicationDbContext dbContext;

        public ActorSyncRepository(
            IApplicationDbContext dbContext,
            ServerDescriptor serverDescriptor,
            IMessageBrokerServiceClient messageBrokerServiceClient) : base(
                serverDescriptor, messageBrokerServiceClient)
        {
            this.dbContext = dbContext;
        }

        public override IMongoCollection<Actor> Collection => dbContext.Actors;

        public override FilterDefinition<Actor> DeleteFilter(Guid id) => FilterDef.Eq(a => a.Id, id);
        public override FilterDefinition<Actor> GetFilter(Guid id) => FilterDef.Eq(a => a.Id, id);
        public override FilterDefinition<Actor> UpdateGetFilter(Guid id) => FilterDef.Eq(a => a.Id, id);
        public override UpdateDefinition<Actor> UpdateFilter(Actor entity)
        {
            return UpdateDef.Set(a => a.FullName, entity.FullName)
                   .Set(a => a.BirthDate, entity.BirthDate)
                   .Set(a => a.BirthYear, entity.BirthYear)
                   .Set(a => a.ImageLink, entity.ImageLink)
                   .Set(a => a.Description, entity.Description);
        }

        public override Actor CreateModelFromId(Guid id)
        {
            return new Actor
            {
                Id = id
            };
        }

        protected static FilterDefinitionBuilder<Actor> FilterDef => Builders<Actor>.Filter;
        protected static UpdateDefinitionBuilder<Actor> UpdateDef => Builders<Actor>.Update;
    }
}
