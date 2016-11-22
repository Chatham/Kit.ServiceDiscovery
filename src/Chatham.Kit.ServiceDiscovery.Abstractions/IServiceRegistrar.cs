using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceRegistrar
    {
        Task Register(CancellationToken ct = default(CancellationToken));
        Task Deregister(CancellationToken ct = default(CancellationToken));
    }
}
