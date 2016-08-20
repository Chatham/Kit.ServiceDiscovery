using Chatham.ServiceDiscovery.Abstractions;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulSubscriberFactory : ISubscriberFactory
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;

        public ConsulSubscriberFactory(ILogger log, IConsulClient client)
        {
            _log = log;
            _client = client;
        }

        public ISubscriber CreateSubscriber(string serviceName, SubscriberOptions options = null)
        {
            return new ConsulSubscriber(_log, _client, serviceName, options?.Tags, options?.DataCenter);
        }
    }

}
