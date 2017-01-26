using Chatham.Kit.ServiceDiscovery.Abstractions;
using Consul;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriberFactory : IConsulServiceSubscriberFactory
    {
        private readonly IConsulClient _consulClient;

        public ConsulServiceSubscriberFactory(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public IServiceSubscriber CreateConsulSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, bool watch = false)
        {
            return new ConsulServiceSubscriber(_consulClient, serviceName, consulOptions, watch);
        }
    }
}