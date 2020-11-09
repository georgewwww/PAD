using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService service;

        public MovieController(IMovieService service)
        {
            this.service = service;
        }

        [HttpGet("{id}")]
        public async Task<Movie> Get(Guid id, CancellationToken cancellationToken)
        {
            return await service.Get(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IList<Movie>> Get(CancellationToken cancellationToken)
        {
            return await service.Get(cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResponse> Post([FromBody] Movie movie,
            CancellationToken cancellationToken)
        {
            return await service.Insert(movie, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<IActionResponse> Put(Guid id, [FromBody] Movie movie,
            CancellationToken cancellationToken)
        {
            movie.Id = id;

            return await service.Update(movie, cancellationToken);
        }

        [HttpDelete]
        public async Task<IActionResponse> Delete(Guid id, CancellationToken cancellationToken)
        {
            return await service.Delete(id, cancellationToken);
        }
    }
}
