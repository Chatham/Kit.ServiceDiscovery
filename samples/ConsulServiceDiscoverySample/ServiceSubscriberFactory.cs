using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Chatham.Kit.ServiceDiscovery.Consul;
using Chatham.Kit.ServiceDiscovery.Throttle;
using Consul;
using Microsoft.Extensions.Logging;

namespace ConsulServiceDiscoverySample
{
    public interface IServiceSubscriberFactory
    {
        IPollingServiceSubscriber CreateSubscriber(string serviceName);
        IPollingServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions,
            ThrottleSubscriberOptions throttleOptions);
    }

    public class ServiceSubscriberFactory : IServiceSubscriberFactory
    {
        private readonly IConsulClient _client;
        private readonly ICacheClient _cache;

        public ServiceSubscriberFactory(IConsulClient consulClient, ICacheClient cache)
        {
            _client = consulClient;
            _cache = cache;
        }

        public IPollingServiceSubscriber CreateSubscriber(string servicName)
        {
            return CreateSubscriber(servicName, ConsulSubscriberOptions.Default, ThrottleSubscriberOptions.Default);
        }

        public IPollingServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, ThrottleSubscriberOptions throttleOptions)
        {
            var consulSubscriber = new ConsulServiceSubscriber(_client, serviceName, consulOptions.Tags, consulOptions.PassingOnly, true);
            var throttleSubscriber = new ThrottleServiceSubscriber(consulSubscriber, throttleOptions.MaxUpdatesPerPeriod, throttleOptions.MaxUpdatesPeriod);
            return new CacheServiceSubscriber(throttleSubscriber, _cache);
        }
    }
}
