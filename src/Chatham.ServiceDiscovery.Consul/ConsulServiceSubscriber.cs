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
            var expandedTags = string.Join(",", _tags);
            var queryOptions = new QueryOptions
            {
                WaitIndex = _waitIndex
            };

            var servicesTask = _client.Health.Service(_serviceName, expandedTags, _passingOnly, queryOptions);
            var services = servicesTask.Result;

            var serviceUris = new List<Uri>();
            foreach (var service in services.Response)
            {
                var host = !string.IsNullOrWhiteSpace(service.Service.Address)
                    ? service.Service.Address
                    : service.Node.Address;
                var builder = new UriBuilder("http", host, service.Service.Port);
                serviceUris.Add(builder.Uri);
            }

            _waitIndex = services.LastIndex;
            _cache.Set(_id, serviceUris);
        }
    }
}
