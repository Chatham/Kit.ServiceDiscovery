using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.ServiceDiscovery.Consul.Core.Tests
{
    [TestClass]
    public class ConsulAgentAdapterTests
    {
        [TestMethod]
        public async Task FetchEndPoints_withoutData_returnsEmptyList()
        {
            var fixture = new ConsulAgentAdapterFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();
            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>();
            fixture.ClientQueryResult.Response = new ServiceEntry[0];

            fixture.SetHealthEndpoint();
            var adapter = fixture.CreateSut();

            var actual = await adapter.FetchEndpoints();
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public async Task FetchEndPoints_withLotsOfData_returnsList()
        {
            var fixture = new ConsulAgentAdapterFixture();
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

            var adapter = fixture.CreateSut();
            var actual = await adapter.FetchEndpoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(services.Count, actual.Count);
        }

        [TestMethod]
        public async Task FetchEndPoints_withMultipleTags_callsConsulWithFirstTagOnly()
        {
            var fixture = new ConsulAgentAdapterFixture();
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
                            Port = 123,
                            Tags = new string[0]
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

            var adapter = fixture.CreateSut();
            await adapter.FetchEndpoints();

            await fixture.HealthEndpoint.Received()
                .Service(Arg.Any<string>(), fixture.Tags[0],
                    Arg.Any<bool>(), Arg.Any<QueryOptions>(), Arg.Any<CancellationToken>());
        }

        [TestMethod]
        public async Task FetchEndPoints_withMultipleTagsAndMultipleMatches_returnsOnlyMatchesWithAllTags()
        {
            var fixture = new ConsulAgentAdapterFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();


            fixture.Tags = new List<string>();
            fixture.Tags.Add(Guid.NewGuid().ToString());
            fixture.Tags.Add(Guid.NewGuid().ToString());

            var superSetOfTags = new List<string>(fixture.Tags);
            superSetOfTags.Add(Guid.NewGuid().ToString());
            
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
                            Port = 123,
                            Tags = new string[0]
                        }
                    },
                    new ServiceEntry
                    {
                        Node = new Node
                        {
                            Address = Guid.NewGuid().ToString()
                        },
                        Service = new AgentService
                        {
                            Address = Guid.NewGuid().ToString(),
                            Port = 123,
                            Tags = new [] { fixture.Tags[0] }
                        }
                    },
                    new ServiceEntry
                    {
                        Node = new Node
                        {
                            Address = Guid.NewGuid().ToString()
                        },
                        Service = new AgentService
                        {
                            Address = Guid.NewGuid().ToString(),
                            Port = 123,
                            Tags = new [] { fixture.Tags[1] }
                        }
                    },
                    new ServiceEntry
                    {
                        Node = new Node
                        {
                            Address = Guid.NewGuid().ToString()
                        },
                        Service = new AgentService
                        {
                            Address = Guid.NewGuid().ToString(),
                            Port = 123,
                            Tags = fixture.Tags.ToArray() // MATCH
                        }
                    },
                    new ServiceEntry
                    {
                        Node = new Node
                        {
                            Address = Guid.NewGuid().ToString()
                        },
                        Service = new AgentService
                        {
                            Address = Guid.NewGuid().ToString(),
                            Port = 123,
                            Tags = superSetOfTags.ToArray() // MATCH
                        }
                    }
                };

            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>
            {
                Response = services.ToArray()
            };

            fixture.SetHealthEndpoint();

            var adapter = fixture.CreateSut();
            var actual = await adapter.FetchEndpoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod]
        public async Task FetchEndPoints_withMultipleTagsAndNoMatches_returnsEmptyList()
        {
            var fixture = new ConsulAgentAdapterFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>
            {
                Response = new ServiceEntry[0]
            };

            fixture.SetHealthEndpoint();
            fixture.Tags = new List<string>();
            fixture.Tags.Add(Guid.NewGuid().ToString());
            fixture.Tags.Add(Guid.NewGuid().ToString());

            var adapter = fixture.CreateSut();
            var actual = await adapter.FetchEndpoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Endpoints_withMultipleTags_onlyReturnsServicesWithAllMatchingTags()
        {
            //Assert.Inconclusive();
        }

        [TestMethod]
        public async Task FetchEndPoints_withoutServiceAddressInReturnedData_buildsUriWithNodeAddressInstead()
        {
            var fixture = new ConsulAgentAdapterFixture();
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

            var adapter = fixture.CreateSut();
            var actual = await adapter.FetchEndpoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual[0].Host == services[0].Node.Address);
        }

        [TestMethod]
        public async Task FetchEndPoints_withBothServiceAddressAndAddressInReturnedData_buildsUriWithServiceAddress()
        {
            var fixture = new ConsulAgentAdapterFixture();
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

            var adapter = fixture.CreateSut();
            var actual = await adapter.FetchEndpoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual[0].Host == services[0].Service.Address);
        }
    }
}
