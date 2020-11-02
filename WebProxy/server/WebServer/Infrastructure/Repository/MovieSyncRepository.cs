using MessageBroker;
using MongoDB.Driver;
using System;
using WebServer.Application;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;
using WebServer.Domain.Events;

namespace WebServer.Infrastructure.Repository
{
    public class MovieSyncRepository : SyncRepository<Movie, MovieEventEntity>, IMovieRepository
    {
        private readonly IApplicationDbContext dbContext;

        public MovieSyncRepository(
            IApplicationDbContext dbContext,
            MessageBus messageBus,
            ServerDescriptor serverDescriptor) : base(
                messageBus, serverDescriptor)
        {
            this.dbContext = dbContext;
        }

        public override IMongoCollection<Movie> Collection => dbContext.Movies;

        public override FilterDefinition<Movie> DeleteFilter(Guid id) => FilterDef.Eq(a => a.Id, id);
        public override FilterDefinition<Movie> GetFilter(Guid id) => FilterDef.Eq(a => a.Id, id);
        public override FilterDefinition<Movie> UpdateGetFilter(Guid id) => FilterDef.Eq(a => a.Id, id);
        public override UpdateDefinition<Movie> UpdateFilter(Movie entity)
        {
            return UpdateDef.Set(a => a.Name, entity.Name)
                .Set(a => a.PosterLink, entity.PosterLink)
                .Set(a => a.PremYear, entity.PremYear)
                .Set(a => a.Genre, entity.Genre)
                .Set(a => a.Time, entity.Time)
                .Set(a => a.Score, entity.Score)
                .Set(a => a.Description, entity.Description);
        }

        public override Movie CreateModel(MovieEventEntity entityEvent)
        {
            return new Movie
            {
                Id = entityEvent.Id,
                Name = entityEvent.Name,
                PosterLink = entityEvent.PosterLink,
                Genre = entityEvent.Genre,
                PremYear = entityEvent.PremYear,
                Time = entityEvent.Time,
                Score = entityEvent.Score,
                Description = entityEvent.Description
            };
        }

        public override MovieEventEntity CreateEventModel(Movie entity)
        {
            return new MovieEventEntity
            {
                Id = entity.Id,
                Name = entity.Name,
                PosterLink = entity.PosterLink,
                Genre = entity.Genre,
                PremYear = entity.PremYear,
                Time = entity.Time,
                Score = entity.Score,
                Description = entity.Description
            };
        }

        public override void UpdateEntity(MovieEventEntity @event, Movie entity, bool copyId)
        {
            if (copyId)
            {
                entity.Id = @event.Id;
            }

            entity.Name = @event.Name;
            entity.PosterLink = @event.PosterLink;
            entity.PremYear = @event.PremYear;
            entity.Score = @event.Score;
            entity.Time = @event.Time;
            entity.Genre = @event.Genre;
            entity.Description = @event.Description;
        }

        protected static FilterDefinitionBuilder<Movie> FilterDef => Builders<Movie>.Filter;
        protected static UpdateDefinitionBuilder<Movie> UpdateDef => Builders<Movie>.Update;

    }
}
