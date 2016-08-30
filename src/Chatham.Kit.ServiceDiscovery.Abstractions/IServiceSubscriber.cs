using System;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber : IStatelessServiceSubscriber, IDisposable
    {
        Task StartSubscription();
        event EventHandler OnSubscriberChange;
    }
}
