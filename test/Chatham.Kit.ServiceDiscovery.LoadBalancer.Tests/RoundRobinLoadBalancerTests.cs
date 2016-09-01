using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using NSubstitute;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.LoadBalancer.Tests
{
    [ExcludeFromCodeCoverage]
    public class RoundRobinLoadBalancerTests
    {
        [Fact]
        public async Task Endpoint_ZeroEndpoints_ReturnsNull()
        {
            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            var lb = new RoundRobinLoadBalancer(subscriber);

            var actual = await lb.Endpoint();

            Assert.Null(actual);
        }

        [Fact]
        public async Task Endpoint_OneEndpoint_ReturnsEndpoint()
        {
            var endpoint = new Endpoint {Host = Guid.NewGuid().ToString(), Port = 123};

            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint> { endpoint }));
            var lb = new RoundRobinLoadBalancer(subscriber);

            var actual = await lb.Endpoint();

            Assert.Equal(endpoint.Host, actual.Host);
            Assert.Equal(endpoint.Port, actual.Port);
        }

        [Fact]
        public async Task Endpoint_MultipleEndpoints_ReturnsEndpointInOrder()
        {
            var expectedList  = new List<Endpoint>
            {
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 1},
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 2},
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 3}
            };

            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(expectedList));
            var lb = new RoundRobinLoadBalancer(subscriber);

            foreach (var expected in expectedList)
            {
                var actual = await lb.Endpoint();
                Assert.Equal(expected.Host, actual.Host);
                Assert.Equal(expected.Port, actual.Port);
            }

            var actualReset = await lb.Endpoint();
            Assert.Equal(expectedList[0].Host, actualReset.Host);
            Assert.Equal(expectedList[0].Port, actualReset.Port);
        }

        [Fact]
        public async Task Endpoint_ResetsNumberOfEndpoints_ReturnsEndpointAndResets()
        {
            var expectedList  = new List<Endpoint>
            {
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 1},
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 2},
                new Endpoint {Host = Guid.NewGuid().ToString(), Port = 3}
            };

            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(expectedList));
            var lb = new RoundRobinLoadBalancer(subscriber);

            foreach (var expected in expectedList.Take(2))
            {
                var actual = await lb.Endpoint();
                Assert.Equal(expected.Host, actual.Host);
                Assert.Equal(expected.Port, actual.Port);
            }

            subscriber.Endpoints().Returns(Task.FromResult(expectedList.Take(2).ToList()));
            var actualReset = await lb.Endpoint();
            Assert.Equal(expectedList[0].Host, actualReset.Host);
            Assert.Equal(expectedList[0].Port, actualReset.Port);
        }
    }
}
