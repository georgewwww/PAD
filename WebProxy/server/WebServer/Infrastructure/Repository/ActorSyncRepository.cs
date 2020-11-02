using MessageBroker;
using MongoDB.Driver;
using System;
using WebServer.Application;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;
using WebServer.Domain.Events;

namespace WebServer.Infrastructure.Repository
{
    public class ActorSyncRepository : SyncRepository<Actor, ActorEventEntity>, IActorRepository
    {
        private readonly IApplicationDbContext dbContext;

        public ActorSyncRepository(
            IApplicationDbContext dbContext,
            MessageBus messageBus,
            ServerDescriptor serverDescriptor) : base(
                messageBus, serverDescriptor)
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

        public override Actor CreateModel(ActorEventEntity entityEvent)
        {
            return new Actor
            {
                Id = entityEvent.Id,
                FullName = entityEvent.FullName,
                ImageLink = entityEvent.ImageLink,
                BirthDate = entityEvent.BirthDate,
                BirthYear = entityEvent.BirthYear,
                Description = entityEvent.Description
            };
        }

        public override ActorEventEntity CreateEventModel(Actor entity)
        {
            return new ActorEventEntity
            {
                Id = entity.Id,
                FullName = entity.FullName,
                ImageLink = entity.ImageLink,
                BirthDate = entity.BirthDate,
                BirthYear = entity.BirthYear,
                Description = entity.Description
            };
        }

        public override void UpdateEntity(ActorEventEntity @event, Actor entity, bool copyId)
        {
            if (copyId)
            {
                entity.Id = @event.Id;
            }

            entity.FullName = @event.FullName;
            entity.BirthDate = @event.BirthDate;
            entity.BirthYear = @event.BirthYear;
            entity.Description = @event.Description;
            entity.ImageLink = @event.ImageLink;
        }

        protected static FilterDefinitionBuilder<Actor> FilterDef => Builders<Actor>.Filter;
        protected static UpdateDefinitionBuilder<Actor> UpdateDef => Builders<Actor>.Update;
    }
}
