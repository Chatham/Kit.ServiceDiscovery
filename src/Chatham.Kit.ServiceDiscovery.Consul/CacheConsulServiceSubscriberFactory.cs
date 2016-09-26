using System;
using System.Threading;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class CacheConsulServiceSubscriberFactory : ICacheServiceSubscriberFactory
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

        public ICacheServiceSubscriber CreateSubscriber(string serviceName)
        {
            return CreateSubscriber(serviceName, ServiceSubscriberOptions.Default);
        }

        public ICacheServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options)
        {
            var consulSubscriber = new ConsulServiceSubscriber(_client, serviceName, options.Tags, options.PassingOnly, true);
            var throttle = new Throttle(5, TimeSpan.FromSeconds(10));
            return new CacheServiceSubscriber(_loggerFactory, consulSubscriber, _cache, throttle);
        }
    }
}
