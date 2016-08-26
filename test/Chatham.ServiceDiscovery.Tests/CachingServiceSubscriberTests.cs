using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Chatham.ServiceDiscovery.Abstractions;
using Chatham.ServiceDiscovery.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.ServiceDiscovery.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CachingServiceSubscriberTests
    {
        [TestMethod]
        public async Task Endpoints_PopulatesCacheImmediately()
        {
            var logger = Substitute.For<ILogger>();
            var serviceSubscriber = Substitute.For<IServiceSubscriber>();
            var cache = Substitute.For<ICacheClient>();
            var throttle = Substitute.For<IThrottle>();

            serviceSubscriber.Endpoints().Returns(Task.FromResult(new List<Uri>()));
            throttle.Queue(Arg.Any<Func<Task<List<Uri>>>>(), Arg.Any<CancellationToken>())
                .Returns(t =>
                {
                    Task.Delay(5000);
                    return Task.FromResult(new List<Uri>());
                });
            //Cache.Get<List<Uri>>(Arg.Any<string>()).Returns(new List<Uri>());
            cache.Set(Arg.Any<object>(), Arg.Any<List<Uri>>()).Returns(new List<Uri>());

            var cts = new CancellationTokenSource();
            var target = new CachingServiceSubscriber(logger, serviceSubscriber, cache, throttle, cts);

            var actual = await target.Endpoints();

            cache.Set(Arg.Any<object>(), Arg.Any<List<Uri>>()).Received(1);
            cache.Get<List<Uri>>(Arg.Any<string>()).Received(1);
        }

        public void Endpoints_StartsSubscriptionLoop() { }

        public void Endpoints_throttlesCallsInSubscriptionLoop() { }

        public void Dispose_cancelsAndDisposesTokenSource() { }

        public void Endpoints_whenCallerCancellationTokenCancels_doesSomething() { }
        
    }
}