using Chatham.Kit.ServiceDiscovery.Abstractions;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public interface IConsulServiceSubscriberFactory
    {
        IServiceSubscriber CreateConsulSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, bool watch);
    }
}