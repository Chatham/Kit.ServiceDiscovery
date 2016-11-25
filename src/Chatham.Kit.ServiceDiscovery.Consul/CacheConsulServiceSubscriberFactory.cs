using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Chatham.Kit.ServiceDiscovery.Throttle;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class CacheConsulServiceSubscriberFactory : IPollingServiceSubscriberFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConsulClient _client;
        private readonly ICacheClient _cache;

        public CacheConsulServiceSubscriberFactory(ILoggerFactory loggerFactory, IConsulClient consulClient, ICacheClient cache)
        {
            _loggerFactory = loggerFactory;
            _client = consulClient;
            _cache = cache;
        }

        public IPollingServiceSubscriber CreateSubscriber(string serviceName)
        {
            return CreateSubscriber(serviceName, ServiceSubscriberOptions.Default);
        }

        public IPollingServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options)
        {
            var consulSubscriber = new ConsulServiceSubscriber(_client, serviceName, options.Tags, options.PassingOnly, true);
            var throttleSubscriber = new ThrottleServiceSubscriber(consulSubscriber, options.MaxUpdatesPerPeriod, options.MaxUpdatesPeriod);
            return new CacheServiceSubscriber(_loggerFactory, throttleSubscriber, _cache);
        }
    }
}
