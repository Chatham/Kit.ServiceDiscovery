using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.Fixed.Tests
{
    public class FixedSubscriberTests
    {
        [Fact]
        public async Task CreatedWithZeroEndpoints_ReturnsEmptyList()
        {
            var subscriber = new FixedSubscriber(new List<Endpoint>());
            var endpoints = await subscriber.Endpoints();
            Assert.Empty(endpoints);
        }

        [Fact]
        public async Task CreatedWithOneEndpoints_ReturnsListWithOneEndpoint()
        {
            var endpoint = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 12345
            };
            var subscriber = new FixedSubscriber(new List<Endpoint> { endpoint });
            var endpoints = await subscriber.Endpoints();
            Assert.NotEmpty(endpoints);
            Assert.Equal(1, endpoints.Count);
            Assert.Equal(endpoint, endpoints[0]);
        }

        [Fact]
        public async Task CreatedWithMultipleEndpoints_ReturnsListWithOneEndpoint()
        {
            var endpoint1 = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 12345
            };
            var endpoint2 = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 54321
            };
            var subscriber = new FixedSubscriber(new List<Endpoint> { endpoint1, endpoint2 });
            var endpoints = await subscriber.Endpoints();
            Assert.NotEmpty(endpoints);
            Assert.Equal(2, endpoints.Count);
            Assert.Equal(new List<Endpoint> {endpoint1, endpoint2}, endpoints);
        }
    }
}
