using System;
using System.Diagnostics.CodeAnalysis;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Consul;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    [ExcludeFromCodeCoverage]
    public static class ConsulServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, 
            ConsulClientConfiguration config = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (config == null)
            {
                config = new ConsulClientConfiguration();
            }

            services.AddCacheServiceDiscovery();

            services.TryAdd(new ServiceDescriptor(typeof(IConsulClient), p => new ConsulClient(config), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IMemoryCache), p => new MemoryCache(new MemoryCacheOptions()), ServiceLifetime.Transient));
            services.TryAddSingleton<ICacheServiceSubscriberFactory, ConsulCacheServiceSubscriberFactory>();

            return services;
        }
    }
}
