using System;
using System.Threading;
using Chatham.ServiceDiscovery.Abstractions;
using Chatham.ServiceDiscovery.Consul.Internal;
using Consul;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

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
            var endpointRetriever = new ConsulEndpointRetriever(_client, serviceName, options.Tags, options.OnlyPassing,
                cts.Token);
            return new ConsulServiceSubscriber(_log, _cache, cts, ct, serviceName, endpointRetriever);
        }
    }
}
