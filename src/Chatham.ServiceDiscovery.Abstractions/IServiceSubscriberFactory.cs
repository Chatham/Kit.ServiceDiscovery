namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriberFactory
    {
        IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options = null);
    }
}
