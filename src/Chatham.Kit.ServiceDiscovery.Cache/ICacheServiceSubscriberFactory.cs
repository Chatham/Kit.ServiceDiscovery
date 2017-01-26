using Chatham.Kit.ServiceDiscovery.Abstractions;

namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public interface ICacheServiceSubscriberFactory
    {
        IPollingServiceSubscriber CreateSubscriber(IServiceSubscriber serviceSubscriber);
    }
}
