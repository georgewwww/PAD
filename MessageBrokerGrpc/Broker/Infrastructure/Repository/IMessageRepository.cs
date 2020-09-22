using Broker.Models;

namespace Broker.Infrastructure.Repository
{
    public interface IMessageRepository
    {
        void Add(Message message);
        Message GetNext();
        bool IsEmpty();
    }
}
