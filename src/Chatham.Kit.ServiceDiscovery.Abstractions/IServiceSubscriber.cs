using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber
    {
        string ServiceName { get; }
        Task<List<Endpoint>> Endpoints();
        Task<List<Endpoint>> Endpoints(CancellationToken ct);
    }
}
