using System;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddSingleton<IConsulServiceSubscriberFactory, ConsulServiceSubscriberFactory>();
            services.AddSingleton<IConsulServiceRegistrarFactory, ConsulServiceRegistrarFactory>();

            return services;
        }
    }
}
