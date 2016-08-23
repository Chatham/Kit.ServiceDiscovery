using System.Collections.Generic;
using System.Threading;
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
        
        public QueryResult<ServiceEntry[]> ClientQueryResult { get; set; }
        public IHealthEndpoint HealthEndpoint { get; set; }

        public ConsulSubscriberFixture()
        {
            Log = Substitute.For<ILogger>();
            Client = Substitute.For<IConsulClient>();
            Cache = Substitute.For<IMemoryCache>();
            HealthEndpoint = Substitute.For<IHealthEndpoint>();
        }

        public void SetHealthEndpoint()
        {
            HealthEndpoint.Service(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<QueryOptions>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(ClientQueryResult));

            Client.Health.Returns(HealthEndpoint);
        }

        public ConsulServiceSubscriber CreateSut()
        {
            return new ConsulServiceSubscriber(Log, Client, Cache, ServiceName, Tags, OnlyPassing);
        }
    }
}

