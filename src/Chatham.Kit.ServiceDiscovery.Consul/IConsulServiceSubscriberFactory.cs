using Chatham.Kit.ServiceDiscovery.Abstractions;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public interface IConsulServiceSubscriberFactory
    {
        IServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, bool watch);
    }
}