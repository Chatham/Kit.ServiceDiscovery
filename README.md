# Kit - Service Discovery [![Build status](https://ci.appveyor.com/api/projects/status/76paor2p95qipdtr/branch/master?svg=true)](https://ci.appveyor.com/project/chatham/kit-servicediscovery/branch/master)

Kit is a .NET Core toolkit for microservices that is heavily inspired by [go-kit](https://gokit.io/). The goal of Kit is to provide a common set of abstractions for platform components of a microservice framework.

The Service Discovery packages provide a set of absractions for pluggable service discovery into the toolkit.

## Packages

* [Absractions](src/Chatham.Kit.ServiceDiscovery.Absractions) - Common absractions for service discovery
* [Cache](src/Chatham.Kit.ServiceDiscovery.Cache) - A caching/throttling discovery provider that can be chained with other discovery providers
* [Consul](src/Chatham.Kit.ServiceDiscovery.Consul) - A [consul](https://www.consul.io/) service discovery provider
* [LoadBalancer](src/Chatham.Kit.ServiceDiscovery.LoadBalancer) - Add load balancing algorithms to service discovery providers

## Example

A full working example can be found in the [samples](samples/) directory.  

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Consul;

namespace ConsulServiceDiscoverySample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsulServiceDiscovery();
        }

        public void Configure(IApplicationBuilder app, ICacheServiceSubscriberFactory cacheServiceSubscriberFactory)
        {
            var serviceSubscriber = cacheServiceSubscriberFactory.CreateSubscriber("FooService");

            app.Run(async context =>
            {
                var endpoints = await serviceSubscriber.Endpoints();
                await context.Response.WriteAsync(string.Join(",", endpoints));
            });
        }
    }
}
```

## Contributing

Please see [CONTRIBUTING.md](/CONTRIBUTING.md).
