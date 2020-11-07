using Common.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Data.Common;
using System.Text;

namespace MessageBus.Services
{
    public class QueueService
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;

        public QueueService(string connectionString)
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
        }

        public void Publish(string queue, object message)
        {
            //QueueDeclare(queue);
            
            var serializedObject = JsonConvert.SerializeObject(message);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queue,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(serializedObject));
        }

        private void QueueDeclare(string queue)
        {
            channel.QueueDeclare(queue: queue);
        }
    }
}
