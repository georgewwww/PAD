using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Infrastructure.Repository
{
    public class MovieSyncRepository : SyncRepository<Movie>, IMovieRepository
    {
        private readonly IApplicationDbContext dbContext;

        public MovieSyncRepository(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var filter = FilterDef.Eq(a => a.Id, id);
            await dbContext.Movies.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<Movie> Get(Guid id, CancellationToken cancellationToken)
        {
            var filter = FilterDef.Eq(a => a.Id, id);
            var actors = await dbContext.Movies.FindAsync(filter, cancellationToken: cancellationToken);
            return actors.FirstOrDefault();
        }

        public async Task<IList<Movie>> Get(CancellationToken cancellationToken)
        {
            var movies = await dbContext.Movies.FindAsync(a => true, cancellationToken: cancellationToken);
            return movies.ToList();
        }

        public async Task<Movie> Insert(Movie entity, CancellationToken cancellationToken)
        {
            await dbContext.Movies.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return new Movie
            {
                Id = entity.Id,
                Name = entity.Name,
                PosterLink = entity.PosterLink,
                PremYear = entity.PremYear,
                Genre = entity.Genre,
                Time = entity.Time,
                Score = entity.Score,
                Description = entity.Description
            };
        }

        public async Task<Movie> Update(Movie entity, CancellationToken cancellationToken)
        {
            var updateDefinition = UpdateDef.Set(a => a.Name, entity.Name)
                .Set(a => a.PosterLink, entity.PosterLink)
                .Set(a => a.PremYear, entity.PremYear)
                .Set(a => a.Genre, entity.Genre)
                .Set(a => a.Time, entity.Time)
                .Set(a => a.Score, entity.Score)
                .Set(a => a.Description, entity.Description);

            var result = await dbContext.Movies.UpdateOneAsync(FilterDef.Eq(a => a.Id, entity.Id),
                updateDefinition,
                cancellationToken: cancellationToken);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return new Movie
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    PosterLink = entity.PosterLink,
                    PremYear = entity.PremYear,
                    Genre = entity.Genre,
                    Time = entity.Time,
                    Score = entity.Score,
                    Description = entity.Description
                };
            }
            else
            {
                return null;
            }
        }

        protected static FilterDefinitionBuilder<Movie> FilterDef => Builders<Movie>.Filter;
        protected static UpdateDefinitionBuilder<Movie> UpdateDef => Builders<Movie>.Update;
    }
}
