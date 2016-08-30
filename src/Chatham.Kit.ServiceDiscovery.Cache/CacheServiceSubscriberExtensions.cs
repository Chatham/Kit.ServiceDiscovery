using System;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public static class CacheServiceSubscriberExtensions
    {
        public static IServiceCollection AddCacheServiceDiscovery(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddTransient<ICacheClient, CacheClient>();

            return services;
        }
    }
}
