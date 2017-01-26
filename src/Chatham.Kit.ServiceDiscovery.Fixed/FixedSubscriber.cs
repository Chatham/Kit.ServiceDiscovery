using Chatham.Kit.ServiceDiscovery.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Fixed
{
    public class FixedSubscriber : IServiceSubscriber
    {
        private readonly List<Endpoint> _endpoints;

        public FixedSubscriber(List<Endpoint> endpoints)
        {
            _endpoints = endpoints;
        }
        
        public Task<List<Endpoint>> Endpoints(CancellationToken ct = default(CancellationToken))
        {
            return Task.FromResult(_endpoints);
        }

        public void Dispose()
        {
        }
    }
}
