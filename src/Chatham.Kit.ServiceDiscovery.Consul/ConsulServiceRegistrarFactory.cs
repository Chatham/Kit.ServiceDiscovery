using Chatham.Kit.ServiceDiscovery.Abstractions;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulServiceRegistrarFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConsulClient _client;

        public ConsulServiceRegistrarFactory(ILoggerFactory loggerFactory, IConsulClient client)
        {
            _loggerFactory = loggerFactory;
            _client = client;
        }

        public IServiceRegistrar CreateConsulRegistrar(AgentServiceRegistration service)
        {
            return new ConsulServiceRegistrar(_loggerFactory, _client, service);
        }
    }
}
