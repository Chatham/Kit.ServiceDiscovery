using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.ServiceDiscovery.Consul.Tests
{
    [TestClass]
    public class ConsulSubscriberTests
    {
        [TestMethod]
        public async Task EndPoints_withoutData_returnsEmptyList()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();
            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>();
            fixture.ClientQueryResult.Response = new ServiceEntry[0];

            fixture.SetHealthEndpoint();
            var subscriber = fixture.CreateSut();

            var actual = await subscriber.EndPoints();
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public async Task EndPoints_withLotsOfData_returnsList()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<ServiceEntry>();
            for (var i = 0; i < 5; i++)
            {
                services.Add(new ServiceEntry
                {
                    Node = new Node
                    {
                        Address = Guid.NewGuid().ToString()
                    },
                    Service = new AgentService
                    {
                        Address = Guid.NewGuid().ToString(),
                        Port = 123
                    }
                });
            }

            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>
            {
                Response = services.ToArray()
            };

            fixture.SetHealthEndpoint();

            var subscriber = fixture.CreateSut();
            var actual = await subscriber.EndPoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(services.Count, actual.Count);
        }

        [TestMethod]
        public async Task EndPoints_withMultipleTags_passesSingleTagToConsul()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<ServiceEntry>
            {
                new ServiceEntry
                {
                    Node = new Node
                    {
                        Address = Guid.NewGuid().ToString()
                    },
                    Service = new AgentService
                    {
                        Address = Guid.NewGuid().ToString(),
                        Port = 123
                    }
                }
            };

            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>
            {
                Response = services.ToArray()
            };

            fixture.SetHealthEndpoint();
            fixture.Tags = new List<string>();
            fixture.Tags.Add(Guid.NewGuid().ToString());
            fixture.Tags.Add(Guid.NewGuid().ToString());
            fixture.Tags.Add(Guid.NewGuid().ToString());

            var subscriber = fixture.CreateSut();
            await subscriber.EndPoints();

            await fixture.HealthEndpoint.Received()
                .Service(Arg.Any<string>(), fixture.Tags[0],
                    Arg.Any<bool>(), Arg.Any<QueryOptions>(), Arg.Any<CancellationToken>());
        }

        [TestMethod]
        public void Endpoints_withMultipleTags_onlyReturnsServicesWithAllMatchingTags()
        {
            //Assert.Inconclusive();
        }

        [TestMethod]
        public async Task EndPoints_withoutServiceAddressInReturnedData_buildsUriWithNodeAddressInstead()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<ServiceEntry>
            {
                new ServiceEntry
                {
                    Node = new Node
                    {
                        Address = Guid.NewGuid().ToString()
                    },
                    Service = new AgentService
                    {
                        Port = 123
                    }
                }
            };

            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>
            {
                Response = services.ToArray()
            };

            fixture.SetHealthEndpoint();

            var subscriber = fixture.CreateSut();
            var actual = await subscriber.EndPoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual[0].Host == services[0].Node.Address);
        }

        [TestMethod]
        public async Task EndPoints_withBothServiceAddressAndAddressInReturnedData_buildsUriWithServiceAddress()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<ServiceEntry>
            {
                new ServiceEntry
                {
                    Node = new Node
                    {
                        Address = Guid.NewGuid().ToString()
                    },
                    Service = new AgentService
                    {
                        Address = Guid.NewGuid().ToString(),
                        Port = 123
                    }
                }
            };

            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>
            {
                Response = services.ToArray()
            };

            fixture.SetHealthEndpoint();

            var subscriber = fixture.CreateSut();
            var actual = await subscriber.EndPoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual[0].Host == services[0].Service.Address);
        }
    }
}
