using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber
    {
        string ServiceName { get; }
        event EventHandler OnSubscriberChange;

        Task<List<Endpoint>> Endpoints();
    }
}
