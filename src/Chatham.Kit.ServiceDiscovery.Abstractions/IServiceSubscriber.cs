using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber : IDisposable
    {
        string ServiceName { get; }

        Task<List<Endpoint>> Endpoints(CancellationToken ct = default(CancellationToken));
    }
}
