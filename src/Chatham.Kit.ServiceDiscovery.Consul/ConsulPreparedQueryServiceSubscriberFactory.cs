using Chatham.Kit.ServiceDiscovery.Abstractions;
using Consul;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulPreparedQueryServiceSubscriberFactory : IConsulPreparedQueryServiceSubscriberFactory
    {
        private readonly IConsulClient _consulClient;

        public ConsulPreparedQueryServiceSubscriberFactory(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public IServiceSubscriber CreateSubscriber(string queryName)
        {
            return new ConsulPreparedQueryServiceSubscriber(_consulClient, queryName);
        }
    }
}
