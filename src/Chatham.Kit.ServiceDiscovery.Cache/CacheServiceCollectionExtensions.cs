using System;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
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

            services.AddMemoryCache();
            services.TryAddSingleton<ICacheClient, CacheClient>();
            services.TryAddSingleton<ICacheServiceSubscriberFactory, CacheServiceSubscriberFactory>();

            return services;
        }
    }
}
