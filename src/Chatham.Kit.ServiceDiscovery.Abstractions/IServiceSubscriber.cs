using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber : IDisposable
    {
        Task<List<Endpoint>> Endpoints(CancellationToken ct = default(CancellationToken));
    }
}
