using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface ILoadBalancer
    {
        Task<Endpoint> Endpoint(CancellationToken ct = default(CancellationToken));
    }
}