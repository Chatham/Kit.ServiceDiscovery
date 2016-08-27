using System.Threading;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriberFactory
    {
        IServiceSubscriber CreateSubscriber(string serviceName);
        IServiceSubscriber CreateSubscriber(string serviceName, CancellationToken ct);
        IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options);
        IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options, CancellationToken ct);
    }
}
