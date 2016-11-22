using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Consul;
using Microsoft.Extensions.Logging;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulServiceRegistrar : IServiceRegistrar
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;
        private readonly AgentServiceRegistration _serviceRegistration;

        public ConsulServiceRegistrar(ILoggerFactory log, IConsulClient client, AgentServiceRegistration serviceRegistration)
        {
            _log = log.CreateLogger(nameof(ConsulServiceRegistrar));
            _client = client;
            _serviceRegistration = serviceRegistration;
        }

        public async Task Register(CancellationToken ct)
        {
            var registrationResult = await _client.Agent.ServiceRegister(_serviceRegistration, ct);
            if (IsSuccessfulStatus(registrationResult.StatusCode))
            {
                _log.LogInformation(
                    $"Succesfully registered service {_serviceRegistration.Name} at {_serviceRegistration.Address}:{_serviceRegistration.Port}");
            }
            else
            {
                throw new Exception($"Unable to register {_serviceRegistration.Name} at {_serviceRegistration.Address}:{_serviceRegistration.Port}");
            }
        }

        public async Task Deregister(CancellationToken ct)
        {
            var deregistrationResult = await _client.Agent.ServiceDeregister(_serviceRegistration.ID, ct);
            if (IsSuccessfulStatus(deregistrationResult.StatusCode))
            {
                _log.LogInformation(
                    $"Deregistered service {_serviceRegistration.Name} [{_serviceRegistration.ID}]");
            }
            else
            {
                throw new Exception($"Unable to deregister {_serviceRegistration.Name}  [{_serviceRegistration.ID}]");
            }
        }

        private static bool IsSuccessfulStatus(HttpStatusCode code)
        {
            return (int)code >= 200 && (int)code < 300;
        }
    }
}
