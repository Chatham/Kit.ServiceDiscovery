using System;
using System.Threading;
using Chatham.ServiceDiscovery.Abstractions;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.ServiceDiscovery.Consul.Simple
{
    public class ConsulSimpleServiceSubscriberFactory : ServiceSubscriberFactory
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;

        public ConsulSimpleServiceSubscriberFactory(ILoggerFactory log, IConsulClient consulClient)
        {
            _log = log.CreateLogger(nameof(ConsulSimpleServiceSubscriberFactory));
            _client = consulClient;
        }

        public override IServiceSubscriber CreateSubscriber(string serviceName, ServiceSubscriberOptions options, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
