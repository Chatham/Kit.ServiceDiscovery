using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber
    {
        string ServiceName { get; }
        event EventHandler OnSubscriberChange;

        Task<List<Uri>> Endpoints();
    }
}
