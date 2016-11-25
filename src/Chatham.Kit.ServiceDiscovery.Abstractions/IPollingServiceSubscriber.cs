using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IPollingServiceSubscriber : IServiceSubscriber
    {
        Task StartSubscription(CancellationToken ct = default(CancellationToken));

        event EventHandler EndpointsChanged;
    }
}
