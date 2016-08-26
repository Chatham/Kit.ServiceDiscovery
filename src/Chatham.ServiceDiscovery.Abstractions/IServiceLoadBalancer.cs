using System;
using System.Threading.Tasks;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface IServiceLoadBalancer
    {
        Task<Uri> Endpoint();
    }
}