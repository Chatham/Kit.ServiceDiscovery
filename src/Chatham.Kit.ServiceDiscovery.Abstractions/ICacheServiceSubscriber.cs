using System;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface ICacheServiceSubscriber : IServiceSubscriber, IDisposable
    {
        Task StartSubscription();
        event EventHandler OnSubscriberChange;
    }
}
