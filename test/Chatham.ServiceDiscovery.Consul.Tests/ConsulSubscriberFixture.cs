using System.Collections.Generic;
using Consul;
using NSubstitute;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Chatham.ServiceDiscovery.Consul.Tests
{
    public class ConsulSubscriberFixture
    {
        public ILogger Log { get; set; }
        public IConsulClient Client { get; set; }
        public IMemoryCache Cache { get; set; }
        public string ServiceName { get; set; }
        public List<string> Tags { get; set; }
        public bool? OnlyPassing { get; set; }

        public QueryResult<CatalogService[]> ClientQueryResult { get; set; }
        public ICatalogEndpoint CatalogEndpoint { get; set; }

        public ConsulSubscriberFixture()
        {
            Log = Substitute.For<ILogger>();
            Client = Substitute.For<IConsulClient>();
            Cache = Substitute.For<IMemoryCache>();
            CatalogEndpoint = Substitute.For<ICatalogEndpoint>();
        }

        public void SetCatalogEndpoint()
        {
            CatalogEndpoint.Service(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<QueryOptions>())
                .Returns(Task.FromResult(ClientQueryResult));

            Client.Catalog.Returns(CatalogEndpoint);
        }

        public ConsulServiceSubscriber CreateSut()
        {
            return new ConsulServiceSubscriber(Log, Client, Cache, ServiceName, Tags, OnlyPassing);
        }
    }
}

