using System;
using System.Threading;
using Chatham.ServiceDiscovery.Abstractions;
using Consul;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriberFactory : IServiceSubscriberFactory
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public ConsulServiceSubscriberFactory(ILoggerFactory log, IConsulClient client)
        {
            _log = log.CreateLogger(typeof(ConsulServiceSubscriberFactory).Namespace);
            _client = client;
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
            return new ConsulServiceSubscriber(_log, _client, _cache, ct, serviceName, options.Tags, options.OnlyPassing);
        }
    }
}
