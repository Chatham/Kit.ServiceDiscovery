using System;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public static class CacheServiceSubscriberExtensions
    {
        public static IServiceCollection AddCacheServiceSubscriber(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(new ServiceDescriptor(typeof(IMemoryCache), p => new MemoryCache(new MemoryCacheOptions()), ServiceLifetime.Transient));
            services.TryAddTransient<ICacheClient, CacheClient>();

            return services;
        }
    }
}
