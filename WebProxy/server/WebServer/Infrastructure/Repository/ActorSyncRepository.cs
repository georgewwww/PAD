using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Infrastructure.Repository
{
    public class ActorSyncRepository : SyncRepository<Actor>, IActorRepository
    {
        private readonly IApplicationDbContext dbContext;

        public ActorSyncRepository(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var filter = FilterDef.Eq(a => a.Id, id);
            await dbContext.Actors.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<Actor> Get(Guid id, CancellationToken cancellationToken)
        {
            var filter = FilterDef.Eq(a => a.Id, id);
            var actors = await dbContext.Actors.FindAsync(filter, cancellationToken: cancellationToken);
            return actors.FirstOrDefault();
        }

        public async Task<IList<Actor>> Get(CancellationToken cancellationToken)
        {
            var actors = await dbContext.Actors.FindAsync(a => true, cancellationToken: cancellationToken);
            return actors.ToList();
        }

        public async Task<Actor> Insert(Actor entity, CancellationToken cancellationToken)
        {
            await dbContext.Actors.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return new Actor
            {
                Id = entity.Id,
                FullName = entity.FullName,
                BirthDate = entity.BirthDate,
                BirthYear = entity.BirthYear,
                ImageLink = entity.ImageLink,
                Description = entity.Description
            };
        }

        public async Task<Actor> Update(Actor entity, CancellationToken cancellationToken)
        {
            var updateDefinition = UpdateDef.Set(a => a.FullName, entity.FullName)
                .Set(a => a.BirthDate, entity.BirthDate)
                .Set(a => a.BirthYear, entity.BirthYear)
                .Set(a => a.ImageLink, entity.ImageLink)
                .Set(a => a.Description, entity.Description);

            var result = await dbContext.Actors.UpdateOneAsync(FilterDef.Eq(a => a.Id, entity.Id),
                updateDefinition,
                cancellationToken: cancellationToken);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return new Actor
                {
                    Id = entity.Id,
                    FullName = entity.FullName,
                    BirthDate = entity.BirthDate,
                    BirthYear = entity.BirthYear,
                    ImageLink = entity.ImageLink,
                    Description = entity.Description
                };
            } else
            {
                return null;
            }
        }

        protected static FilterDefinitionBuilder<Actor> FilterDef => Builders<Actor>.Filter;
        protected static UpdateDefinitionBuilder<Actor> UpdateDef => Builders<Actor>.Update;
    }
}
