using System;
using System.Threading;
using Chatham.ServiceDiscovery.Abstractions;

namespace Chatham.ServiceDiscovery
{
    public class ServiceSubscriberFactory : IServiceSubscriberFactory
    {
        public IServiceSubscriber CreateSubscriber(string serviceName)
        {
            return CreateSubscriber(serviceName, ServiceSubscriberOptions.Default);
        }

        public IServiceSubscriber CreateSubscriber(string serviceName, CancellationToken ct)
        {
            return CreateSubscriber(serviceName, ServiceSubscriberOptions.Default, ct);
        }

        public IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options)
        {
            return CreateSubscriber(serviceName, options, CancellationToken.None);
        }

        public virtual IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
