using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Chatham.ServiceDiscovery.Abstractions;
using Chatham.ServiceDiscovery.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.ServiceDiscovery.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CachingServiceSubscriberTests
    {
        [TestMethod]
        public async Task Endpoints_PopulatesCacheImmediately()
        {
            var Logger = Substitute.For<ILogger>();
            var ServiceSubscriber = Substitute.For<IServiceSubscriber>();
            var Cache = new MemoryCache(new MemoryCacheOptions());
            var Throttle = Substitute.For<IThrottle>();

            ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<Uri>()));
            Throttle.Queue(Arg.Any<Func<Task<List<Uri>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new List<Uri>()));

            var cts = new CancellationTokenSource();
            var target = new CachingServiceSubscriber(Logger, ServiceSubscriber, Cache, Throttle, cts);

            

            var actual = await target.Endpoints();

            //Cache.Set(Arg.Any<string>(), Arg.Any<List<Uri>>()).Received(1);
            //Cache.Get(Arg.Any<string>()).Received(1);
        }

        public void Endpoints_StartsSubscriptionLoop() { }

        public void Endpoints_throttlesCallsInSubscriptionLoop() { }

        public void Dispose_cancelsAndDisposesTokenSource() { }

        public void Endpoints_whenCallerCancellationTokenCancels_doesSomething() { }
        
    }
}