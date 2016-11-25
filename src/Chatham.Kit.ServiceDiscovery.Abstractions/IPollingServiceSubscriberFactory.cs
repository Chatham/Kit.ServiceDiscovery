namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IPollingServiceSubscriberFactory
    {
        IPollingServiceSubscriber CreateSubscriber(string serviceName);
        IPollingServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options);
    }
}
