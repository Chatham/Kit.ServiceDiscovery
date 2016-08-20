using System;
using System.Collections.Generic;
using System.Linq;
using Consul;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.ServiceDiscovery.Consul.Tests
{
    [TestClass]
    public class ConsulSubscriberTests
    {
        [TestMethod]
        public void EndPoints_withoutData_returnsEmptyList()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();
            fixture.ClientQueryResult = new QueryResult<CatalogService[]>();
            fixture.ClientQueryResult.Response = new CatalogService[0];

            fixture.SetCatalogEndpoint();
            var subscriber = fixture.CreateSut();

            var actual = subscriber.EndPoints();
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void EndPoints_withLotsOfData_returnsList()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<CatalogService>();
            for (var i = 0; i < 5; i++)
            {
                services.Add(new CatalogService()
                {
                    Address = Guid.NewGuid().ToString(),
                    ServiceAddress = Guid.NewGuid().ToString(),
                    ServicePort = 123
                });
            }

            fixture.ClientQueryResult = new QueryResult<CatalogService[]>
            {
                Response = services.ToArray()
            };

            fixture.SetCatalogEndpoint();

            var subscriber = fixture.CreateSut();
            var actual = subscriber.EndPoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(services.Count, actual.Count);
        }

        [ExpectedException(typeof (Exception))]
        [TestMethod]
        public void EndPoints_consulThrowsException_throwsException()
        {
            var expectedException = new Exception();

            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            fixture.ClientQueryResult = new QueryResult<CatalogService[]>();

            fixture.Client.Catalog.Returns(x => { throw expectedException; });
            var subscriber = fixture.CreateSut();

            try
            {
                subscriber.EndPoints();
            }
            catch (Exception actualException)
            {
                Assert.AreEqual(expectedException, actualException);
                throw;
            }
        }

        [TestMethod]
        public void EndPoints_withDatacenter_passesDatacenterToConsul()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<CatalogService>()
            {
                new CatalogService()
                {
                    Address = Guid.NewGuid().ToString(),
                    ServiceAddress = Guid.NewGuid().ToString(),
                    ServicePort = 123
                }
            };

            fixture.ClientQueryResult = new QueryResult<CatalogService[]>
            {
                Response = services.ToArray()
            };

            fixture.SetCatalogEndpoint();
            fixture.DataCenter = Guid.NewGuid().ToString();

            var subscriber = fixture.CreateSut();
            subscriber.EndPoints();

            fixture.CatalogEndpoint.Received()
                .Service(Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Is<QueryOptions>(x => x.Datacenter == fixture.DataCenter));
        }

        [TestMethod]
        public void EndPoints_withTags_passesTagsToConsul()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<CatalogService>()
            {
                new CatalogService()
                {
                    Address = Guid.NewGuid().ToString(),
                    ServiceAddress = Guid.NewGuid().ToString(),
                    ServicePort = 123
                }
            };

            fixture.ClientQueryResult = new QueryResult<CatalogService[]>
            {
                Response = services.ToArray()
            };

            fixture.SetCatalogEndpoint();
            fixture.Tags = new List<string>();
            fixture.Tags.Add(Guid.NewGuid().ToString());
            fixture.Tags.Add(Guid.NewGuid().ToString());
            fixture.Tags.Add(Guid.NewGuid().ToString());

            var subscriber = fixture.CreateSut();

            subscriber.EndPoints();

            fixture.CatalogEndpoint.Received()
                .Service(Arg.Any<string>(), Arg.Is<string>(x => x.Split(',').Count() == fixture.Tags.Count),
                    Arg.Any<QueryOptions>());
        }

        [TestMethod]
        public void EndPoints_withoutServiceAddressInReturnedData_buildsUriWithAddressInstead()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<CatalogService>()
            {
                new CatalogService()
                {
                    Address = Guid.NewGuid().ToString(),
                    ServiceAddress = null,
                    ServicePort = 123
                }
            };

            fixture.ClientQueryResult = new QueryResult<CatalogService[]>
            {
                Response = services.ToArray()
            };

            fixture.SetCatalogEndpoint();

            var subscriber = fixture.CreateSut();
            var actual = subscriber.EndPoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual[0].Host == services[0].Address);
        }

        [TestMethod]
        public void EndPoints_withBothServiceAddressAndAddressInReturnedData_buildsUriWithServiceAddress()
        {
            var fixture = new ConsulSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();

            var services = new List<CatalogService>()
            {
                new CatalogService()
                {
                    Address = Guid.NewGuid().ToString(),
                    ServiceAddress = Guid.NewGuid().ToString(),
                    ServicePort = 123
                }
            };

            fixture.ClientQueryResult = new QueryResult<CatalogService[]>
            {
                Response = services.ToArray()
            };

            fixture.SetCatalogEndpoint();

            var subscriber = fixture.CreateSut();
            var actual = subscriber.EndPoints();

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual[0].Host == services[0].ServiceAddress);
        }
    }
}
