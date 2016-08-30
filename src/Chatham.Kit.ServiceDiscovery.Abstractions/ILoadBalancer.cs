using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface ILoadBalancer
    {
        Task<Endpoint> Endpoint();
    }
}