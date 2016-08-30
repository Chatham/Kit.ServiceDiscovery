using System;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Consul;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public static class ConsulServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, ConsulConfiguration config = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var consulConfig = new ConsulClientConfiguration();
            if (config?.Address != null)
            {
                consulConfig.Address = config.Address;
            }
            if (config?.Token != null)
            {
                consulConfig.Token = config.Token;
            }

            services.AddCacheServiceDiscovery();

            services.TryAdd(new ServiceDescriptor(typeof(IConsulClient), p => new ConsulClient(consulConfig), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IMemoryCache), p => new MemoryCache(new MemoryCacheOptions()), ServiceLifetime.Transient));
            services.TryAddSingleton<IServiceSubscriberFactory, ConsulServiceSubscriberFactory>();

            return services;
        }
    }
}
