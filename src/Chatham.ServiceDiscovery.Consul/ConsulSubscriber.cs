using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Consul;
using Chatham.ServiceDiscovery.Abstractions;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulSubscriber : ISubscriber
    {
        private readonly ILogger _log;
        private readonly IConsulClient _client;
        private readonly string _serviceName;
        private readonly List<string> _tags;
        private readonly string _dataCenter;

        public ConsulSubscriber(ILogger log, IConsulClient client, string serviceName, List<string> tags = null, string dataCenter = null)
        {
            _log = log;
            _client = client;
            _serviceName = serviceName;
            _tags = tags ?? new List<string>();
            _dataCenter = dataCenter;
        }

        public List<Uri> EndPoints()
        {
            var expandedTags = string.Join(",", _tags);
            var queryOptions = new QueryOptions();
            if (_dataCenter != null)
            {
                queryOptions.Datacenter = _dataCenter;
            }

            var servicesTask = _client.Catalog.Service(_serviceName, expandedTags, queryOptions);
            var services = servicesTask.Result;

            var serviceUris = new List<Uri>();

            foreach (var service in services.Response)
            {
                var host = !string.IsNullOrWhiteSpace(service.ServiceAddress)
                    ? service.ServiceAddress
                    : service.Address;
                var builder = new UriBuilder("http", host, service.ServicePort);
                serviceUris.Add(builder.Uri);
            }

            return serviceUris;
        }
    }
}
