namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface ISubscriberFactory
    {
        ISubscriber CreateSubscriber(string serviceName, SubscriberOptions options = null);
    }
}
