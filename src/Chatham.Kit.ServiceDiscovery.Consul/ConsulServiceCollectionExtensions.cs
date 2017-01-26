using System;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public static class ConsulServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<IConsulClient, ConsulClient>();
            services.AddSingleton<IConsulServiceSubscriberFactory, ConsulServiceSubscriberFactory>();
            services.AddSingleton<IConsulServiceRegistrarFactory, ConsulServiceRegistrarFactory>();

            return services;
        }
    }
}
