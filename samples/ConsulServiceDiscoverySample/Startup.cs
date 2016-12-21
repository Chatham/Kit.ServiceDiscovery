using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chatham.Kit.ServiceDiscovery.Consul;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConsulServiceDiscoverySample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsulServiceDiscovery();
            services.TryAddSingleton<IServiceSubscriberFactory, ServiceSubscriberFactory>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, 
            IServiceSubscriberFactory subscriberFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                 app.UseDeveloperExceptionPage();
            }

            var log = loggerFactory.CreateLogger(nameof(Startup));
            var serviceSubscriber = subscriberFactory.CreateSubscriber("FooService");
            serviceSubscriber.StartSubscription();
            serviceSubscriber.EndpointsChanged += async (sender, eventArgs) =>
            {
                // Reset connection pool, do something with this info, etc

                var endpoints = await serviceSubscriber.Endpoints();
                var services = string.Join(",", endpoints);
                log.LogInformation($"Received updated subscribers [{services}]");
            };

            app.Run(async context =>
            {
                var endpoints = await serviceSubscriber.Endpoints();
                await context.Response.WriteAsync(string.Join(",", endpoints));
            });
        }
    }
}
