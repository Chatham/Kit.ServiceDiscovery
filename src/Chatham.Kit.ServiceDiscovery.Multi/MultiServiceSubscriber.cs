using Chatham.Kit.ServiceDiscovery.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Multi
{
    public class MultiServiceSubscriber : IServiceSubscriber
    {
        private readonly List<IServiceSubscriber> _serviceSubscribers;

        public MultiServiceSubscriber(List<IServiceSubscriber> serviceSubscribers)
        {
            _serviceSubscribers = serviceSubscribers;
        }

        public async Task<List<Endpoint>> Endpoints(CancellationToken ct = default(CancellationToken))
        {
            foreach (var subscriber in _serviceSubscribers)
            {
                var endpoints = await subscriber.Endpoints(ct).ConfigureAwait(false);
                if (endpoints.Count > 0)
                {
                    return endpoints;
                }
            }
            return new List<Endpoint>();
        }

        public void Dispose()
        {
            foreach (var subscriber in _serviceSubscribers)
            {
                subscriber.Dispose();
            }
        }
    }
}
