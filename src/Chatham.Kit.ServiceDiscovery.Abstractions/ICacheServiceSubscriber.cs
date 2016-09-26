using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface ICacheServiceSubscriber : IServiceSubscriber, IDisposable
    {
        Task StartSubscription();
        Task StartSubscription(CancellationToken ct);

        event EventHandler EndpointsChanged;
    }
}
