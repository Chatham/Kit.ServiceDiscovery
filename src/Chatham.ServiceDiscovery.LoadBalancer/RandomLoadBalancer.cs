using System;
using System.Threading.Tasks;
using Chatham.ServiceDiscovery.Abstractions;

namespace Chatham.ServiceDiscovery.LoadBalancer
{
    public class RandomLoadBalancer : IServiceLoadBalancer
    {
        private readonly Random _random;
        private readonly IServiceSubscriber _subscriber;

        public RandomLoadBalancer(IServiceSubscriber subscriber, int? seed = null)
        {
            _subscriber = subscriber;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

         public async Task<Uri> Endpoint()
        {
            var endpoints = await _subscriber.Endpoints();
            if (endpoints == null || endpoints.Count == 0)
            {
                return null;
            }

            return endpoints[_random.Next(endpoints.Count) - 1];
        }
    }
}
