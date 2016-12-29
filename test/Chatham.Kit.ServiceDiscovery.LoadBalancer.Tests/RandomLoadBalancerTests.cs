using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using NSubstitute;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.LoadBalancer.Tests
{
    public class RandomLoadBalancerTests
    {
        [Fact]
        public async Task Endpoint_ZeroEndpoints_ReturnsNull()
        {
            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            var lb = new RandomLoadBalancer(subscriber);

            var actual = await lb.Endpoint();

            Assert.Null(actual);
        }

        [Fact]
        public async Task Endpoint_OneEndpoint_ReturnsEndpoint()
        {
            var endpoint = new Endpoint { Host = Guid.NewGuid().ToString(), Port = 123 };

            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint> { endpoint }));
            var lb = new RoundRobinLoadBalancer(subscriber);

            var actual = new List<Endpoint>
            {
                await lb.Endpoint(),
                await lb.Endpoint()
            };

            foreach (var endpointActual in actual)
            {
                Assert.Equal(endpoint.Host, endpointActual.Host);
                Assert.Equal(endpoint.Port, endpointActual.Port);
            }
        }

        [Fact]
        public async Task Endpoint_MultipleEndpoints_ReturnsEndpointInRandomOrder()
        {
            var expectedList = new List<Endpoint>
            {
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 1},
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 2},
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 3}
            };

            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(expectedList));
            var lb = new RandomLoadBalancer(subscriber, 0);


            var actual = new List<Endpoint>();
            for (var i = 0; i < 10; i++)
            {
                actual.Add(await lb.Endpoint());
            }

            var expectedOrder = new List<int> {2, 2, 2, 1, 0, 1, 2, 1, 2, 0};
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expectedList[expectedOrder[i]].Host, actual[i].Host);
                Assert.Equal(expectedList[expectedOrder[i]].Port, actual[i].Port);
            }
        }
    }
}
