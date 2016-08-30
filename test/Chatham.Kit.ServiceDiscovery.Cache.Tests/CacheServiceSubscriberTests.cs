using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache.Utilities;
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
            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<ServiceEndpoint>()));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<ServiceEndpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(t =>
                {
                    Task.Delay(5000);
                    return Task.FromResult(new List<ServiceEndpoint>());
                });

            var subscriber = fixture.CreateSut();
            await subscriber.Endpoints();

            fixture.Cache.Received(1).Set(Arg.Any<object>(), Arg.Any<List<ServiceEndpoint>>());
            fixture.Cache.Received(1).Get<List<ServiceEndpoint>>(Arg.Any<string>());
        }

        public void Endpoints_StartsSubscriptionLoop() { }

        public void Endpoints_throttlesCallsInSubscriptionLoop() { }

        public void Dispose_cancelsAndDisposesTokenSource() { }

        public void Endpoints_whenCallerCancellationTokenCancels_doesSomething() { }

        [TestMethod]
        public async Task SubscriptionLoop_ReceivesChangedEndpoints_UpdatesCacheAndFiresEvent()
        {
            var result1 = new List<ServiceEndpoint>
                {
                    new ServiceEndpoint
                    {
                        Host = Guid.NewGuid().ToString(),
                        Port = 123
                    }
                };
            var result2 = new List<ServiceEndpoint>
                {
                    new ServiceEndpoint
                    {
                        Host = Guid.NewGuid().ToString(),
                        Port = 321
                    }
                };

            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints()
                .Returns(Task.FromResult(result1));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<ServiceEndpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(result1), Task.FromResult(result2))
                .AndDoes(x => Thread.Sleep(1000));

            var eventWasCalled = false;
            var subscriber = fixture.CreateSut();
            subscriber.OnSubscriberChange += (sender, args) => eventWasCalled = true;

            await subscriber.Endpoints();
            Thread.Sleep(2500);

            Received.InOrder(() =>
            {
                fixture.Cache.Set(Arg.Any<string>(), result1);
                fixture.Cache.Set(Arg.Any<string>(), result2);
            });

            fixture.Cache.Received(2).Set(Arg.Any<string>(), Arg.Any<List<ServiceEndpoint>>());
            await fixture.Throttle.Received(3).Queue(Arg.Any<Func<Task<List<ServiceEndpoint>>>>(), Arg.Any<CancellationToken>());
            Assert.IsTrue(eventWasCalled);
        }

        public void SubscriptionLoop_ReceivesSameEndpoints_DoesNotUpdateCacheAndFireEvent() { }

    }
}