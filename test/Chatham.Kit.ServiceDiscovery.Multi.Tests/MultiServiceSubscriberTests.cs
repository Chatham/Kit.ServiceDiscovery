using System.Collections.Generic;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Fixed;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.Multi.Tests
{
    public class MultiServiceSubscriberTests
    {
        [Fact]
        public async Task SingleSubscriber_ReturnsEndpoints()
        {
            var subscriber = new FixedSubscriber(new List<Endpoint> {new Endpoint()});
            var multiSubscriber = new MultiServiceSubscriber(new List<IServiceSubscriber> { subscriber });
            var endpoints = await multiSubscriber.Endpoints();
            Assert.NotEmpty(endpoints);
            Assert.Equal(1, endpoints.Count);
            Assert.Equal(await subscriber.Endpoints(), endpoints);
        } 
    }
}
