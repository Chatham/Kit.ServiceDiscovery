using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Consul;
using Chatham.ServiceDiscovery.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriber : IServiceSubscriber
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;
        private readonly IMemoryCache _cache;

        private readonly string _serviceName;
        private readonly bool _passingOnly;
        private readonly List<string> _tags;

        private readonly string _id;
        private ulong _waitIndex;

        public ConsulServiceSubscriber(ILogger log, IConsulClient client, IMemoryCache cache, 
            string serviceName, List<string> tags = null, bool? passingOnly = null)
        {
            _log = log;
            _client = client;
            _cache = cache;

            _serviceName = serviceName;
            _passingOnly = passingOnly ?? false;
            _tags = tags ?? new List<string>();

            _id = Guid.NewGuid().ToString();
            _waitIndex = 0;
        }

        public List<Uri> EndPoints()
        {
            UpdateCache();
            return _cache.Get<List<Uri>>(_id);
        }

        private void UpdateCache()
        {
            // Consul doesn't support more than one tag in its service query method.
            // https://github.com/hashicorp/consul/issues/294
            // Hashi suggest prepared queries, but they don't support blocking.
            // https://www.consul.io/docs/agent/http/query.html#execute
            // If we want blocking for efficiency, we must filter tags manually.
            var tag = "";
            if (_tags.Count > 0)
            {
                tag = _tags[0];
            }

            var queryOptions = new QueryOptions
            {
                WaitIndex = _waitIndex
            };
            var servicesTask = _client.Health.Service(_serviceName, tag, _passingOnly, queryOptions);
            var servicesTaskResult = servicesTask.Result;
            var services = servicesTaskResult.Response;

            if (_tags.Count > 0)
            {
                services = FilterByTag(services, _tags);
            }

            var serviceUris = new List<Uri>();
            foreach (var service in services)
            {
                var host = !string.IsNullOrWhiteSpace(service.Service.Address)
                    ? service.Service.Address
                    : service.Node.Address;
                var builder = new UriBuilder("http", host, service.Service.Port);
                serviceUris.Add(builder.Uri);
            }

            _waitIndex = servicesTaskResult.LastIndex;
            _cache.Set(_id, serviceUris);
        }

        private static ServiceEntry[] FilterByTag(ServiceEntry[] entries, List<string> tags)
        {
            //TODO: Filter
            return entries;
        }
    }
}
