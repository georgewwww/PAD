﻿using Newtonsoft.Json;
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

        public MessageBus(string connectionString)
        {
            var dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            connectionFactory = new ConnectionFactory
            {
                HostName = dbConnectionStringBuilder["Host"].ToString(),
                Port = 5672,
                UserName = dbConnectionStringBuilder["Username"].ToString(),
                Password = dbConnectionStringBuilder["Password"].ToString()
            };
            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Publish(string queue, object message)
        {
            QueueDeclare(queue);

            var serializeObject = JsonConvert.SerializeObject(message);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queue,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(serializeObject));
        }

        public void Subscribe<T>(string queue, Action<T> onEvent)
        {
            QueueDeclare(queue);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                var obj = JsonConvert.DeserializeObject<T>(message);
                onEvent(obj);
            };

            channel.BasicConsume(queue: queue,
                autoAck: true,
                consumer: consumer);
        }

        private void QueueDeclare(string queue)
        {
            channel.QueueDeclare(
                queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false);
        }
    }
}
