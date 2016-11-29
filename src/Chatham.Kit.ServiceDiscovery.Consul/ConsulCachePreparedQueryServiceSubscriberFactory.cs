using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Chatham.Kit.ServiceDiscovery.Throttle;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulCachePreparedQueryServiceSubscriberFactory : IPollingServiceSubscriberFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConsulClient _client;
        private readonly ICacheClient _cache;

        public ConsulCachePreparedQueryServiceSubscriberFactory(ILoggerFactory loggerFactory, IConsulClient consulClient, ICacheClient cache)
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
            var consulSubscriber = new ConsulPreparedQueryServiceSubscriber(_client, serviceName);
            var throttleSubscriber = new ThrottleServiceSubscriber(consulSubscriber, options.MaxUpdatesPerPeriod, options.MaxUpdatesPeriod);
            return new CacheServiceSubscriber(throttleSubscriber, _loggerFactory, _cache);
        }
    }
}
