namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriberFactory
    {
        IServiceSubscriber CreateSubscriber(string serviceName);
        IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options);
    }
}
