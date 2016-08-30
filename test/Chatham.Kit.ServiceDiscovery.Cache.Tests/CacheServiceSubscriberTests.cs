using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
                .AndDoes(async x => await Task.Delay(1000));

            var subscriber = fixture.CreateSut();
            await subscriber.Endpoints();

            fixture.Cache.Received(1).Set(Arg.Any<object>(), Arg.Any<List<Endpoint>>());
            fixture.Cache.Received(1).Get<List<Endpoint>>(Arg.Any<string>());
        }

        [TestMethod]
        public async Task StartSubscription_InitialCacheSetThrowsException_ExceptionBubblesUp()
        {
            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));

            var expectedException = new Exception();

            fixture.Cache.Set(Arg.Any<object>(), Arg.Any<List<Endpoint>>()).Throws(expectedException);

            var subscriber = fixture.CreateSut();

            Exception actualException = null;
            try
            {
                await subscriber.StartSubscription();

            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            
            Assert.AreSame(expectedException, actualException);
        }

        [TestMethod]
        public async Task StartSubscription_InitialServiceCallThrowsException_NothingSetInCacheAndExceptionBubblesUp()
        {
            var fixture = new CacheServiceSubscriberFixture();
            var expectedException = new Exception();
            fixture.ServiceSubscriber.Endpoints().Throws(expectedException);

            var subscriber = fixture.CreateSut();

            Exception actualException = null;
            try
            {
                await subscriber.StartSubscription();

            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            Assert.AreSame(expectedException, actualException);

            fixture.Cache.DidNotReceive().Set(Arg.Any<object>(), Arg.Any<List<Endpoint>>());
        }

        [TestMethod]
        public async Task Dispose_CancelsAndDisposesTokenSource()
        {
            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new List<Endpoint>()))
                .AndDoes(x => Thread.Sleep(500));

            var subscriber = fixture.CreateSut();
            await subscriber.Endpoints();

            subscriber.Dispose();

            await Task.Delay(250).ContinueWith(t =>
            {
                Assert.IsTrue(fixture.CancellationTokenSource.IsCancellationRequested);
                fixture.Cache.Received(1).Remove(Arg.Any<string>());
            });
        }

        [TestMethod]
        public async Task Endpoints_CalledWhenDisposed_ThrowsDisposedException()
        {
            var fixture = new CacheServiceSubscriberFixture();
            var subscriber = fixture.CreateSut();
            subscriber.Dispose();

            ObjectDisposedException actualException = null;
            try
            {
                await subscriber.Endpoints();
            }
            catch (ObjectDisposedException ex)
            {
                actualException = ex;
            }
            Assert.IsInstanceOfType(actualException, typeof(ObjectDisposedException));
        }

        [TestMethod]
        public void Dispose_DisposedTwice_DoesNotRemoveFromCacheTwice()
        {
            var fixture = new CacheServiceSubscriberFixture();
            var subscriber = fixture.CreateSut();
            subscriber.Dispose();
            subscriber.Dispose();

            fixture.Cache.Received(1).Remove(Arg.Any<string>());
        }

        [TestMethod]
        public async Task SubscriptionLoop_CancelRequested_CancelsSubscriptionLoop()
        {
            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new List<Endpoint>()))
                .AndDoes(x=> Thread.Sleep(1000));

            var subscriber = fixture.CreateSut();
            await subscriber.Endpoints();

            await Task.Delay(2500);
            await fixture.Throttle.Received(3).Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
            fixture.Throttle.ClearReceivedCalls();

            fixture.CancellationTokenSource.Cancel();

            await Task.Delay(2000);
            await fixture.Throttle.Received(0)
                .Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
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
                .AndDoes(x => Thread.Sleep(500));

            var eventWasCalled = false;
            var subscriber = fixture.CreateSut();
            subscriber.OnSubscriberChange += (sender, args) => eventWasCalled = true;

            await subscriber.Endpoints();
            await Task.Delay(1250).ContinueWith(async t =>
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
                .AndDoes(x => Thread.Sleep(500));

            var eventWasCalled = false;
            var subscriber = fixture.CreateSut();
            subscriber.OnSubscriberChange += (sender, args) => eventWasCalled = true;

            await subscriber.Endpoints();
            await Task.Delay(1250).ContinueWith(async t =>
            {
                fixture.Cache.Received(1).Set(Arg.Any<string>(), Arg.Any<List<Endpoint>>());
                await
                    fixture.Throttle.Received(3)
                        .Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
                Assert.IsFalse(eventWasCalled);
            });
        }
        
        [TestMethod]
        public async Task SubscriptionLoop_ReceivesDifferentCountOfEndpoints_UpdatesCacheAndFiresEvent()
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
                        Port = 789
                    },
                    new Endpoint
                    {
                        Host = Guid.NewGuid().ToString(),
                        Port = 456
                    }
                };

            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints()
                .Returns(Task.FromResult(result1));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(result1), Task.FromResult(result2))
                .AndDoes(x => Thread.Sleep(500));

            var eventWasCalled = false;
            var subscriber = fixture.CreateSut();
            subscriber.OnSubscriberChange += (sender, args) => eventWasCalled = true;

            await subscriber.Endpoints();
            await Task.Delay(1250).ContinueWith(async t =>
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
        public async Task SubscriptionLoop_WithoutUpdateEvent_UpdatesCacheWithoutFiringEventAndDying()
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
                        Port = 789
                    },
                    new Endpoint
                    {
                        Host = Guid.NewGuid().ToString(),
                        Port = 456
                    }
                };

            var fixture = new CacheServiceSubscriberFixture();
            fixture.ServiceSubscriber.Endpoints()
                .Returns(Task.FromResult(result1));
            fixture.Throttle.Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(result1), Task.FromResult(result2))
                .AndDoes(x => Thread.Sleep(500));
            
            var subscriber = fixture.CreateSut();

            await subscriber.Endpoints();
            await Task.Delay(1250).ContinueWith(async t =>
            {
                Received.InOrder(() =>
                {
                    fixture.Cache.Set(Arg.Any<string>(), result1);
                    fixture.Cache.Set(Arg.Any<string>(), result2);
                });

                fixture.Cache.Received(2).Set(Arg.Any<string>(), Arg.Any<List<Endpoint>>());
                await fixture.Throttle.Received(3).Queue(Arg.Any<Func<Task<List<Endpoint>>>>(), Arg.Any<CancellationToken>());
            });
        }
        
    }
}