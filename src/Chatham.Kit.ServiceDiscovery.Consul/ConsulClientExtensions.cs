using Chatham.Kit.ServiceDiscovery.Abstractions;
using Consul;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public static class ConsulClientExtensions
    {
        public static Endpoint ToEndpoint(this ServiceEntry serviceEntry)
        {
            var host = !string.IsNullOrWhiteSpace(serviceEntry.Service.Address)
                ? serviceEntry.Service.Address
                : serviceEntry.Node.Address;
            return new Endpoint { Host = host, Port = serviceEntry.Service.Port };
        }
    }
}
