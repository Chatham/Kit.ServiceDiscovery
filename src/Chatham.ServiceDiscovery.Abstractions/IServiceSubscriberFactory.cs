namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriberFactory
    {
        IServiceSubscriber CreateSubscriber(string serviceName, SubscriberOptions option = null);
    }
}
