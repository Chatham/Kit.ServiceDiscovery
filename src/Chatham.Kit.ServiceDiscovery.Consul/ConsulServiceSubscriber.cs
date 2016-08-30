using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Consul;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriber : IServiceSubscriber
    {
        private readonly IConsulClient _client;
        private readonly List<string> _tags;
        private readonly bool _passingOnly;
        private readonly bool _watch;

        internal ulong WaitIndex;
        private readonly CancellationToken _cancellationToken;

        public string ServiceName { get; }

        public ConsulServiceSubscriber(IConsulClient client, string serviceName, List<string> tags,
            bool passingOnly, CancellationToken cancellationToken, bool watch)
        {
            _client = client;

            ServiceName = serviceName;
            _tags = tags ?? new List<string>();
            _passingOnly = passingOnly;

            _cancellationToken = cancellationToken;
            _watch = watch;
        }

        public async Task<List<Endpoint>> Endpoints()
        {
            // Consul doesn't support more than one tag in its service query method.
            // https://github.com/hashicorp/consul/issues/294
            // Hashicorp suggest prepared queries, but they don't support blocking.
            // https://www.consul.io/docs/agent/http/query.html#execute
            // If we want blocking for efficiency, we must filter tags manually.
            var tag = string.Empty;
            if (_tags.Count > 0)
            {
                tag = _tags[0];
            }

            var queryOptions = new QueryOptions
            {
                WaitIndex = WaitIndex
            };
            var servicesTask = await 
                _client.Health.Service(ServiceName, tag, _passingOnly, queryOptions, _cancellationToken)
                    .ConfigureAwait(false);

            if (_tags.Count > 1)
            {
                servicesTask.Response = FilterByTag(servicesTask.Response, _tags);
            }

            if (_watch)
            {
                WaitIndex = servicesTask.LastIndex;
            }

            return CreateEndpointUris(servicesTask.Response);
        }

        private static List<Endpoint> CreateEndpointUris(IEnumerable<ServiceEntry> services)
        {
            var serviceUris = new List<Endpoint>();
            foreach (var service in services)
            {
                var host = !string.IsNullOrWhiteSpace(service.Service.Address)
                    ? service.Service.Address
                    : service.Node.Address;
                var endpoint = new Endpoint { Host = host, Port = service.Service.Port};
                serviceUris.Add(endpoint);
            }
            return serviceUris;
        }

        private static ServiceEntry[] FilterByTag(IEnumerable<ServiceEntry> entries, IReadOnlyCollection<string> tags)
        {
            return entries
                .Where(x => tags.All(x.Service.Tags.Contains))
                .ToArray();
        }
    }
}