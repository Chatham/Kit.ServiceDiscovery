using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.ServiceDiscovery.Consul.Utilities
{
    public interface IThrottle
    {
        Task<T> Queue<T>(Func<T> action, CancellationToken cancel);
    }
}