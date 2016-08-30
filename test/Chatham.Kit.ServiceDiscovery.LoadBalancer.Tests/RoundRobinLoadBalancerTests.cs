using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            var endpoint1 = new Endpoint {Host = Guid.NewGuid().ToString(), Port = 1};
            var endpoint2 = new Endpoint {Host = Guid.NewGuid().ToString(), Port = 2};
            var endpoint3 = new Endpoint {Host = Guid.NewGuid().ToString(), Port = 3};

            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint> { endpoint1, endpoint2, endpoint3 }));
            var lb = new RoundRobinLoadBalancer(subscriber);

            var actual1 = await lb.Endpoint();
            var actual2 = await lb.Endpoint();
            var actual3 = await lb.Endpoint();

            Assert.AreEqual(endpoint1.Host, actual1.Host);
            Assert.AreEqual(endpoint1.Port, actual1.Port);
            Assert.AreEqual(endpoint2.Host, actual2.Host);
            Assert.AreEqual(endpoint2.Port, actual2.Port);
            Assert.AreEqual(endpoint3.Host, actual3.Host);
            Assert.AreEqual(endpoint3.Port, actual3.Port);
        }
    }
}
