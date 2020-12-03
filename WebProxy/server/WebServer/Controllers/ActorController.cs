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
    public class ActorController : ControllerBase
    {
        private readonly IActorService service;

        public ActorController(IActorService service)
        {
            this.service = service;
        }

        [HttpGet("{id}")]
        public async Task<Actor> Get(Guid id, CancellationToken cancellationToken)
        {
            return await service.Get(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IList<Actor>> Get(CancellationToken cancellationToken)
        {
            return await service.Get(cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResponse> Post([FromBody] Actor actor,
            CancellationToken cancellationToken)
        {
            return await service.Insert(actor, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<IActionResponse> Put(Guid id, [FromBody] Actor actor,
            CancellationToken cancellationToken)
        {
            actor.Id = id;

            return await service.Update(actor, cancellationToken);
        }

        [HttpDelete]
        public async Task<IActionResponse> Delete(Guid id, CancellationToken cancellationToken)
        {
            return await service.Delete(id, cancellationToken);
        }
    }
}
