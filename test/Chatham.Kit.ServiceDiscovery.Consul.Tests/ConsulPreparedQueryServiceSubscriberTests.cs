using System;
using System.Threading.Tasks;
using Consul;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.Consul.Tests
{
    public class ConsulPreparedQueryServiceSubscriberTests
    {
        [Fact]
        public async Task Endpoints_withoutData_returnsEmptyList()
        {
            var fixture = new ConsulPreparedQueryServiceSubscriberFixture();
            fixture.ServiceName = Guid.NewGuid().ToString();
            fixture.ClientQueryResult = new QueryResult<PreparedQueryExecuteResponse>();
            fixture.ClientQueryResult.Response = new PreparedQueryExecuteResponse
            {
                Nodes = new ServiceEntry[0]
            };

            fixture.SetPreparedQueryEndpoint();
            var subscriber = fixture.CreateSut();

            var actual = await subscriber.Endpoints();
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Count);
        }
    }
}
