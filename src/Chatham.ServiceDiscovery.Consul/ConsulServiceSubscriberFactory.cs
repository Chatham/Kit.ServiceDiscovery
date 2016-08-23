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
        private readonly IMemoryCache _cache;

        public ConsulServiceSubscriberFactory(ILoggerFactory log, IConsulClient client)
        {
            _log = log.CreateLogger(typeof(ConsulServiceSubscriberFactory).Namespace);
            _client = client;
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options = null)
        {
            return new ConsulServiceSubscriber(_log, _client, _cache, serviceName, options?.Tags, options?.OnlyPassing);
        }
    }
}
