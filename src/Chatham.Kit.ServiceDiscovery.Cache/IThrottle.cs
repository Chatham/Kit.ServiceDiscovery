using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public interface IThrottle : IDisposable
    {
        Task<T> Queue<T>(Func<T> action, CancellationToken ct);
    }
}