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
    public class RandomLoadBalancerTests
    {
        [TestMethod]
        public async Task Endpoint_ZeroEndpoints_ReturnsNull()
        {
            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            var lb = new RandomLoadBalancer(subscriber);

            var actual = await lb.Endpoint();

            Assert.IsNull(actual);
        }


        [TestMethod]
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
                Assert.AreEqual(endpoint.Host, endpointActual.Host);
                Assert.AreEqual(endpoint.Port, endpointActual.Port);
            }
        }
    }
}
