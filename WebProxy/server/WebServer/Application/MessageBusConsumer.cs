using Common.Models;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Application
{
    public class MessageBusConsumer : IConsumer<Request>
    {
        private readonly IActorRepository actorRepository;
        private readonly IMovieRepository movieRepository;

        public MessageBusConsumer(IActorRepository actorRepository,
            IMovieRepository movieRepository)
        {
            this.actorRepository = actorRepository;
            this.movieRepository = movieRepository;
        }

        public async Task Consume(ConsumeContext<Request> context)
        {
            try
            {
                await ExecuteQuery(context.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task ExecuteQuery(Request request)
        {
            var descriptive = request.Descriptive.Split("#");

            if (descriptive.Length != 2) return;

            if (descriptive[1] == "actor")
            {
                var entity = JsonConvert.DeserializeObject<Actor>(request.Payload);
                switch (descriptive[0])
                {
                    case "insert":
                        await actorRepository.Insert(entity, new CancellationTokenSource().Token, false);
                        break;
                    case "update":
                        await actorRepository.Update(entity, new CancellationTokenSource().Token, false);
                        break;
                    case "delete":
                        await actorRepository.Delete(entity.Id, new CancellationTokenSource().Token, false);
                        break;
                }
            }
            else if (descriptive[1] == "movie")
            {
                var entity = JsonConvert.DeserializeObject<Movie>(request.Payload);
                switch (descriptive[0])
                {
                    case "insert":
                        await movieRepository.Insert(entity, new CancellationTokenSource().Token, false);
                        break;
                    case "update":
                        await movieRepository.Update(entity, new CancellationTokenSource().Token, false);
                        break;
                    case "delete":
                        await movieRepository.Delete(entity.Id, new CancellationTokenSource().Token, false);
                        break;
                }
            }
        }
    }
}
