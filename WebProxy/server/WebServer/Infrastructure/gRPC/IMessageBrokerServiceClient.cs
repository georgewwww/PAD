using System.Threading.Tasks;

namespace WebServer.Infrastructure.gRPC
{
    public interface IMessageBrokerServiceClient
    {
        Task Subscribe(string id, string hostName);
        Task Publish(string id, string descriptive, string payload);
    }
}
