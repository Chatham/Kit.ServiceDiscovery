using Chatham.ServiceDiscovery.Abstractions;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriberFactory : IServiceSubscriberFactory
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;

        public ConsulServiceSubscriberFactory(ILoggerFactory log, IConsulClient client)
        {
            _log = log.CreateLogger(typeof(ConsulServiceSubscriberFactory).Namespace);
            _client = client;
        }

        public IServiceSubscriber CreateSubscriber(string serviceName, SubscriberOptions options)
        {
            return new ConsulServiceSubscriber(_log, _client, serviceName, options?.Tags, options?.DataCenter);
        }
    }
}
