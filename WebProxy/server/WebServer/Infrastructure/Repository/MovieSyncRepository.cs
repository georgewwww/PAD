using MongoDB.Driver;
using System;
using WebServer.Application;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;
using WebServer.Infrastructure.gRPC;

namespace WebServer.Infrastructure.Repository
{
    public class MovieSyncRepository : SyncRepository<Movie>, IMovieRepository
    {
        private readonly IApplicationDbContext dbContext;

        public MovieSyncRepository(
            IApplicationDbContext dbContext,
            ServerDescriptor serverDescriptor,
            IMessageBrokerServiceClient messageBrokerServiceClient) : base(
                serverDescriptor, messageBrokerServiceClient)
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

        public override Movie CreateModelFromId(Guid id)
        {
            return new Movie
            {
                Id = id
            };
        }

        protected static FilterDefinitionBuilder<Movie> FilterDef => Builders<Movie>.Filter;
        protected static UpdateDefinitionBuilder<Movie> UpdateDef => Builders<Movie>.Update;

    }
}
