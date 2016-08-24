using System;
using System.Collections.Generic;
using System.Linq;
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

            fixture.CancellationTokenSource.Cancel();
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

            fixture.CancellationTokenSource.Cancel();
        }

        [TestMethod]
        public async Task EndPoints_consulThrowsException_throwsException()
        {
            var expectedException = new Exception();

            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>();

            fixture.Client.Health.Returns(x => { throw expectedException; });
            var subscriber = fixture.CreateSut();

            Action action = async () => await subscriber.EndPoints();
            Assert.ThrowsException<Exception>(action);

            fixture.CancellationTokenSource.Cancel();
        }

        [TestMethod]
        public async Task EndPoints_withTags_passesTagsToConsul()
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
            var _ = await subscriber.EndPoints();

            await fixture.HealthEndpoint.Received()
                .Service(Arg.Any<string>(), Arg.Is<string>(x => x.Split(',').Count() == fixture.Tags.Count),
                    Arg.Any<bool>(), Arg.Any<QueryOptions>(), Arg.Any<CancellationToken>());

            fixture.CancellationTokenSource.Cancel();
        }

        [TestMethod]
        public async Task EndPoints_withoutServiceAddressInReturnedData_buildsUriWithAddressInstead()
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
            Assert.IsTrue(actual[0].Host == services[0].Node.Address);

            fixture.CancellationTokenSource.Cancel();
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

            fixture.CancellationTokenSource.Cancel();
        }
    }
}
