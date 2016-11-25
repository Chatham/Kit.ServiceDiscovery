using System;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
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

            services.AddCacheServiceSubscriber();

            services.TryAdd(new ServiceDescriptor(typeof(IConsulClient), p => new ConsulClient(config), ServiceLifetime.Singleton));
            services.TryAddSingleton<IPollingServiceSubscriberFactory, CacheConsulServiceSubscriberFactory>();
            services.TryAddSingleton<IConsulServiceRegistrarFactory, ConsulServiceRegistrarFactory>();

            return services;
        }
    }
}
