using Broker.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Broker.Infrastructure.Repository
{
    public interface IMessageRepository
    {
        Task Add(Message message, CancellationToken cancellationToken);
        Task<Message> GetNext(CancellationToken cancellationToken);
        Task<bool> IsEmpty(CancellationToken cancellationToken);
    }
}
