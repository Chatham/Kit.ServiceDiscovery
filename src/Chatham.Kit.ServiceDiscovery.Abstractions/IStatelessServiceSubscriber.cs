using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IStatelessServiceSubscriber
    {
        string ServiceName { get; }
        Task<List<Endpoint>> Endpoints();
    }
}
