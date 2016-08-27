using System;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceLoadBalancer
    {
        Task<Uri> Endpoint();
    }
}