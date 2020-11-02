using MessageBroker.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Data.Common;
using System.Text;

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
                Password = dbConnectionStringBuilder["Password"].ToString()
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
            BindQueue(exchange, QueueName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                var obj = JsonConvert.DeserializeObject<T>(message);
                onEvent(obj);
            };

            channel.BasicConsume(queue: QueueName,
                autoAck: true,
                consumer: consumer);
        }

        private void BindQueue(string exchange, string queueName)
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
