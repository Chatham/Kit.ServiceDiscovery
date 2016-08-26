using System;
using System.Threading;
using Chatham.ServiceDiscovery.Abstractions;
using Consul;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Chatham.ServiceDiscovery.Utilities;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriberFactory : IServiceSubscriberFactory
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;
        private readonly IMemoryCache _cache;

        public ConsulServiceSubscriberFactory(ILoggerFactory log, IConsulClient consulClient, IMemoryCache cache)
        {
            _log = log.CreateLogger(typeof(ConsulServiceSubscriberFactory).Namespace);
            _client = consulClient;
            _cache = cache;
        }

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

        public IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options, CancellationToken ct)
        {
            var cts = new CancellationTokenSource();
            var consulSubscriber = new ConsulServiceSubscriber(_client, serviceName, options.Tags, options.PassingOnly,
                cts.Token, true);
            return new CachingServiceSubscriber(_log, _cache, cts, ct, consulSubscriber, new Throttle(5, TimeSpan.FromSeconds(10)));
        }
    }
}
