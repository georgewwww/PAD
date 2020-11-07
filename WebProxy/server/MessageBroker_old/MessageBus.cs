using Common.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker
{
    public class MessageBus
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        public string QueueName { get; }

        public MessageBus(string connectionString)
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
            QueueName = "q_" + Guid.NewGuid().ToString();
        }

        public void Publish(string exchange, object message)
        {
            ExchangeDeclare(exchange);

            var serializeObject = JsonConvert.SerializeObject(message);

            channel.BasicPublish(
                exchange: exchange,
                routingKey: string.Empty,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(serializeObject));
        }

        public void Subscribe<T>(string exchange, Action<T> onEvent)
        {
            QueueDeclare(QueueName);
            ExchangeDeclare(exchange);
            BindQueue<T>(exchange, QueueName);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                Console.WriteLine($"Publish to exchange {exchange} the following " + message);

                var obj = JsonConvert.DeserializeObject<T>(message);

                onEvent(obj);
                await Task.Yield();
            };

            channel.BasicConsume(queue: QueueName,
                autoAck: true,
                consumer: consumer);
        }

        private void BindQueue<T>(string exchange, string queueName)
        {
            channel.QueueBind(
                queue: queueName,
                exchange: exchange,
                routingKey: string.Empty);
        }

        private void QueueDeclare(string queue)
        {
            channel.QueueDeclare(queue: queue);
        }

        private void ExchangeDeclare(string exchange)
        {
            channel.ExchangeDeclare(exchange, ExchangeType.Fanout);
        }
    }
}
