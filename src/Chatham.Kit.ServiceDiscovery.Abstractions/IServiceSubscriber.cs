using System;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber : IStatelessServiceSubscriber, IDisposable
    {
        event EventHandler OnSubscriberChange;
    }
}
