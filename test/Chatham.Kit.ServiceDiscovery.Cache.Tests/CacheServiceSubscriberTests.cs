using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
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
            fixture.ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new List<Endpoint>()))
                .AndDoes(async x => await Task.Delay(5000));

            var subscriber = fixture.CreateSut();
            await subscriber.Endpoints();

            fixture.Cache.Received(1).Set(Arg.Any<object>(), Arg.Any<List<Endpoint>>());
            fixture.Cache.Received(1).Get<List<Endpoint>>(Arg.Any<string>());
        }

        public async Task Dispose_cancelsAndDisposesTokenSource()
        {
            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new List<Endpoint>()))
                .AndDoes(x => Thread.Sleep(1000));

            var subscriber = fixture.CreateSut();
            await subscriber.Endpoints();

            subscriber.Dispose();

        }

        [TestMethod]
        public async Task SubscriptionLoop_CallerCancelsRequest_CancelsSubscriptionLoop()
        {
            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new List<Endpoint>()))
                .AndDoes(x=> Thread.Sleep(1000));

            var subscriber = fixture.CreateSut();
            await subscriber.Endpoints();

            await Task.Delay(2500).ContinueWith(async t1 =>
            {
                await fixture.Throttle.Received(3).Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
                fixture.Throttle.ClearReceivedCalls();
                fixture.CancellationTokenSource.Cancel();

                await Task.Delay(2000).ContinueWith(async t2 =>
                {
                    await fixture.Throttle.Received(0)
                        .Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
                });
            });
        }

        [TestMethod]
        public async Task SubscriptionLoop_ReceivesChangedEndpoints_UpdatesCacheAndFiresEvent()
        {
            var result1 = new List<Endpoint>
                {
                    new Endpoint
                    {
                        Host = Guid.NewGuid().ToString(),
                        Port = 123
                    }
                };
            var result2 = new List<Endpoint>
                {
                    new Endpoint
                    {
                        Host = Guid.NewGuid().ToString(),
                        Port = 321
                    }
                };

            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints()
                .Returns(Task.FromResult(result1));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(result1), Task.FromResult(result2))
                .AndDoes(x => Thread.Sleep(1000));

            var eventWasCalled = false;
            var subscriber = fixture.CreateSut();
            subscriber.OnSubscriberChange += (sender, args) => eventWasCalled = true;

            await subscriber.Endpoints();
            await Task.Delay(2500).ContinueWith(async t =>
            {
                Received.InOrder(() =>
                {
                    fixture.Cache.Set(Arg.Any<string>(), result1);
                    fixture.Cache.Set(Arg.Any<string>(), result2);
                });

                fixture.Cache.Received(2).Set(Arg.Any<string>(), Arg.Any<List<Endpoint>>());
                await fixture.Throttle.Received(3).Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
                Assert.IsTrue(eventWasCalled);
            });
        }

        [TestMethod]
        public async Task SubscriptionLoop_ReceivesSameEndpoints_DoesNotUpdateCacheOrFireEvent()
        {
            var result = new List<Endpoint>
                {
                    new Endpoint
                    {
                        Host = Guid.NewGuid().ToString(),
                        Port = 123
                    }
                };

            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints()
                .Returns(Task.FromResult(result));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(result), Task.FromResult(result))
                .AndDoes(x => Thread.Sleep(1000));

            var eventWasCalled = false;
            var subscriber = fixture.CreateSut();
            subscriber.OnSubscriberChange += (sender, args) => eventWasCalled = true;

            await subscriber.Endpoints();
            Thread.Sleep(2500);

            fixture.Cache.Received(1).Set(Arg.Any<string>(), Arg.Any<List<Endpoint>>());
            await fixture.Throttle.Received(3).Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
            Assert.IsFalse(eventWasCalled);
        }

    }
}