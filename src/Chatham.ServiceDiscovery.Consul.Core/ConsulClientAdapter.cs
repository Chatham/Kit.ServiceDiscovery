using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;

namespace Chatham.ServiceDiscovery.Consul.Core
{
    public class ConsulClientAdapter : IConsulClientAdapter
    {
        private readonly IConsulClient _client;
        private readonly string _serviceName;
        private readonly List<string> _tags;
        private readonly bool _passingOnly;
        private readonly bool _watch;

        public ulong WaitIndex { get; set; }
        private readonly CancellationToken _cancellationToken;

        public ConsulClientAdapter(IConsulClient client, string serviceName, List<string> tags,
            bool passingOnly, CancellationToken cancellationToken, bool watch)
        {
            _client = client;

            _serviceName = serviceName;
            _tags = tags ?? new List<string>();
            _passingOnly = passingOnly;

            _cancellationToken = cancellationToken;
            _watch = watch;
        }

        public async Task<List<Uri>> FetchEndpoints()
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
            var servicesTask = await _client.Health.Service(_serviceName, tag, _passingOnly, queryOptions, _cancellationToken);

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

        private static List<Uri> CreateEndpointUris(ServiceEntry[] services)
        {
            var serviceUris = new List<Uri>();
            foreach (var service in services)
            {
                var host = !string.IsNullOrWhiteSpace(service.Service.Address)
                    ? service.Service.Address
                    : service.Node.Address;
                var builder = new UriBuilder("http", host, service.Service.Port);
                serviceUris.Add(builder.Uri);
            }
            return serviceUris;
        }

        private static ServiceEntry[] FilterByTag(ServiceEntry[] entries, List<string> tags)
        {
            return entries
                .Where(x => tags.All(x.Service.Tags.Contains))
                .ToArray();
        }
    }
}