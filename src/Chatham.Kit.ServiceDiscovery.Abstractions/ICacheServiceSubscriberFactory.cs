using System.Threading;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface ICacheServiceSubscriberFactory
    {
        ICacheServiceSubscriber CreateSubscriber(string serviceName);
        ICacheServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options, CancellationToken ct);
    }
}
