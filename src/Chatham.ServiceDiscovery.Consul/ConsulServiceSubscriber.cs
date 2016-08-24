using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly CancellationToken _cancellationToken;

        private readonly string _serviceName;
        private readonly bool _passingOnly;
        private readonly List<string> _tags;

        private readonly string _id = Guid.NewGuid().ToString();
        private ulong _waitIndex;

        private Task _subscriptionTask;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public ConsulServiceSubscriber(ILogger log, IConsulClient client, IMemoryCache cache, CancellationToken cancellationToken,
            string serviceName, List<string> tags, bool passingOnly)
        {
            _log = log;
            _client = client;
            _cache = cache;
            _cancellationToken = cancellationToken;

            _serviceName = serviceName;
            _passingOnly = passingOnly;
            _tags = tags ?? new List<string>();
        }

        public async Task<List<Uri>> EndPoints()
        {
            await StartSubscription();

            return _cache.Get<List<Uri>>(_id);
        }

        private async Task StartSubscription()
        {
            if (_subscriptionTask == null)
            {
                await _mutex.WaitAsync(_cancellationToken);
                try
                {
                    if (_subscriptionTask == null)
                    {
                        var endpoints = await FetchEndpoints();
                        var serviceUris = CreateEndpointUris(endpoints.Response);

                        _cache.Set(_id, serviceUris);
                        _waitIndex = endpoints.LastIndex;

                        _subscriptionTask = SubscriptionLoop();
                    }
                }
                finally
                {
                    _mutex.Release();
                }
                
            }
        }

        private async Task SubscriptionLoop()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var endpoints = await FetchEndpoints();
                    _log.LogDebug($"Received updated endpoints for {_serviceName}");
                    var serviceUris = CreateEndpointUris(endpoints.Response);

                    _cache.Set(_id, serviceUris);
                    _waitIndex = endpoints.LastIndex;
                }
                catch (TaskCanceledException)
                {
                    _cache.Remove(_id);
                }
            }
        }

        private async Task<QueryResult<ServiceEntry[]>> FetchEndpoints()
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
                WaitIndex = _waitIndex
            };
            var servicesTask = await _client.Health.Service(_serviceName, tag, _passingOnly, queryOptions, _cancellationToken);

            if (_tags.Count > 1)
            {
                servicesTask.Response = FilterByTag(servicesTask.Response, _tags);
            }

            return servicesTask;
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
