using System;
using Chatham.ServiceDiscovery.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chatham.ServiceDiscovery.Consul
{
    public static class ConsulServiceCollectionExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(ServiceDescriptor.Singleton<ISubscriberFactory, ConsulSubscriberFactory>());

            return services;
        }
    }
}
