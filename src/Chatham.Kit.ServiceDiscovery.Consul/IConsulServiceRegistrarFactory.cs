using Chatham.Kit.ServiceDiscovery.Abstractions;
using Consul;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public interface IConsulServiceRegistrarFactory
    {
        IServiceRegistrar CreateConsulRegistrar(AgentServiceRegistration service);
    }
}
