using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.Kit.ServiceDiscovery.LoadBalancer.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RoundRobinLoadBalancerTests
    {
        [TestMethod]
        public async Task Endpoint_ZeroEndpoints_ReturnsNull()
        {
            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            var lb = new RoundRobinLoadBalancer(subscriber);

            var actual = await lb.Endpoint();

            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task Endpoint_OneEndpoint_ReturnsEndpoint()
        {
            var endpoint = new Endpoint {Host = Guid.NewGuid().ToString(), Port = 123};

            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint> { endpoint }));
            var lb = new RoundRobinLoadBalancer(subscriber);

            var actual = await lb.Endpoint();

            Assert.AreEqual(endpoint.Host, actual.Host);
            Assert.AreEqual(endpoint.Port, actual.Port);
        }

        [TestMethod]
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
                Assert.AreEqual(expected.Host, actual.Host);
                Assert.AreEqual(expected.Port, actual.Port);
            }

            var actualReset = await lb.Endpoint();
            Assert.AreEqual(expectedList[0].Host, actualReset.Host);
            Assert.AreEqual(expectedList[0].Port, actualReset.Port);
        }

        [TestMethod]
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
                Assert.AreEqual(expected.Host, actual.Host);
                Assert.AreEqual(expected.Port, actual.Port);
            }

            subscriber.Endpoints().Returns(Task.FromResult(expectedList.Take(2).ToList()));
            var actualReset = await lb.Endpoint();
            Assert.AreEqual(expectedList[0].Host, actualReset.Host);
            Assert.AreEqual(expectedList[0].Port, actualReset.Port);
        }
    }
}
