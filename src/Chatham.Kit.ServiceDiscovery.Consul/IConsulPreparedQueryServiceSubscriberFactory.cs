using Chatham.Kit.ServiceDiscovery.Abstractions;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public interface IConsulPreparedQueryServiceSubscriberFactory
    {
        IServiceSubscriber CreateSubscriber(string queryName);
    }
}
