using Chatham.Kit.ServiceDiscovery.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chatham.Kit.ServiceDiscovery.Consul;

namespace ConsulServiceDiscoverySample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsulServiceDiscovery();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, 
            ICacheServiceSubscriberFactory cacheServiceSubscriberFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                 app.UseDeveloperExceptionPage();
            }

            var log = loggerFactory.CreateLogger(nameof(Startup));
            var serviceSubscriber = cacheServiceSubscriberFactory.CreateSubscriber("FooService");
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
