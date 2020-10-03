using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Broker.Models;

namespace Broker.Infrastructure.Repository
{
    public interface IConnectionRepository
    {
        Task Add(Connection connection, CancellationToken cancellationToken);
        Task Remove(string address, CancellationToken cancellationToken);
        Task<IList<Connection>> GetConnectionsByBank(string bank, CancellationToken cancellationToken);
    }
}
