using Common;
using Common.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Application.Abstractions;
using WebServer.Domain.Entities;

namespace WebServer.Application
{
    public class MessageBrokerReceiver
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private IActorRepository actorRepository;
        private IMovieRepository movieRepository;

        public MessageBrokerReceiver(string connectionString)
        {
            var dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            var host = dbConnectionStringBuilder["Host"].ToString();
            HttpHelper.WaitForPortOpen(1000, host, int.Parse(dbConnectionStringBuilder["Port"].ToString()));

            connectionFactory = new ConnectionFactory
            {
                HostName = host,
                UserName = dbConnectionStringBuilder["Username"].ToString(),
                Password = dbConnectionStringBuilder["Password"].ToString(),
                DispatchConsumersAsync = true
            };
            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Subscribe(string queue, IActorRepository actorRepository, IMovieRepository movieRepository)
        {
            this.actorRepository = actorRepository;
            this.movieRepository = movieRepository;
            QueueDeclare(queue);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                var request = JsonConvert.DeserializeObject<RequestMessage>(message);
                await ExecuteQuery(request);
                await Task.Yield();
            };
        }

        private async Task ExecuteQuery(RequestMessage request)
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

        private void QueueDeclare(string queue)
        {
            channel.QueueDeclare(queue: queue);
        }
    }
}
