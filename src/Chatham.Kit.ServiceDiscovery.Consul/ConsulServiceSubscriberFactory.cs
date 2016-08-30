﻿using System;
using System.Threading;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriberFactory : IServiceSubscriberFactory
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;
        private readonly ICacheClient _cache;

        public ConsulServiceSubscriberFactory(ILoggerFactory log, IConsulClient consulClient, ICacheClient cache)
        {
            _log = log.CreateLogger(typeof(ConsulServiceSubscriberFactory).Namespace);
            _client = consulClient;
            _cache = cache;
        }

        public IServiceSubscriber CreateSubscriber(string serviceName)
        {
            return CreateSubscriber(serviceName, ServiceSubscriberOptions.Default, CancellationToken.None);
        }

        public IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options, CancellationToken ct)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var consulSubscriber = new ConsulServiceSubscriber(_client, serviceName, options.Tags, options.PassingOnly,
                cts.Token, true);
            return new CacheServiceSubscriber(_log, consulSubscriber, _cache, new Throttle(5, TimeSpan.FromSeconds(10)), cts);
        }
    }
}
