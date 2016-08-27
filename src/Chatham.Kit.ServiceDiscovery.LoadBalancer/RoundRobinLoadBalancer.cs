using System;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;

namespace Chatham.ServiceDiscovery.LoadBalancer
{
    public class RoundRobinLoadBalancer : IServiceLoadBalancer
    {
        private readonly IServiceSubscriber _subscriber;
        private readonly object _lock = new object();
        private int _index;

        public RoundRobinLoadBalancer(IServiceSubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        public async Task<Uri> Endpoint()
        {
            var endpoints = await _subscriber.Endpoints();
            if (endpoints == null || endpoints.Count == 0)
            {
                return null;
            }

            lock (_lock)
            {
                if (_index >= endpoints.Count)
                {
                    _index = 0;
                }
                var uri = endpoints[_index];
                _index++;

                return uri;
            }
        }
    }
}
