using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;

namespace Chatham.ServiceDiscovery
{
    //pass-through class
    [ExcludeFromCodeCoverage]
    public class CacheClient : ICacheClient
    {
        private readonly IMemoryCache _cache;

        public CacheClient(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(object key)
        {
            return _cache.Get<T>(key);
        }

        public T Set<T>(object key, T value)
        {
            return _cache.Set(key, value);
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }
    }
}
