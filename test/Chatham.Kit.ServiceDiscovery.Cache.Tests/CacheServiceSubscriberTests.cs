using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.Kit.ServiceDiscovery.Cache.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CacheServiceSubscriberTests
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

            cache.Received(1).Set(Arg.Any<object>(), Arg.Any<List<Uri>>());
            cache.Received(1).Get<List<Uri>>(Arg.Any<string>());
        }

        public void Endpoints_StartsSubscriptionLoop() { }

        public void Endpoints_throttlesCallsInSubscriptionLoop() { }

        public void Dispose_cancelsAndDisposesTokenSource() { }

        public void Endpoints_whenCallerCancellationTokenCancels_doesSomething() { }
        
    }
}