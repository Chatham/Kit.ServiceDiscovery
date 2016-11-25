using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using NSubstitute;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.Throttle.Tests
{
    public class ThrottleTests
    {
        [Fact]
        public async Task Queue_WithSingleActionCall_FiresAction()
        {
            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            var period = new TimeSpan(0, 0, 2);
            var target = new ThrottleServiceSubscriber(subscriber, 1, period);

            var actual = await target.Endpoints();

            Assert.NotNull(actual);
            await subscriber.Received(1).Endpoints(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Queue_WithTwoActionCallsAndLimitOfOnePerPeriod_FiresBothActionsBeforeTwiceTheDelay()
        {
            var subscriber = Substitute.For<IServiceSubscriber>();
            subscriber.Endpoints().Returns(Task.FromResult(new List<Endpoint>()));
            var period = new TimeSpan(0, 0, 2);
            var target = new ThrottleServiceSubscriber(subscriber, 1, period);

            var action = Substitute.For<Func<string>>();
            action().Returns("hello");

            var stopwatch = Stopwatch.StartNew();
            var first = await target.Endpoints();
            var second = await target.Endpoints();

            Assert.NotNull(first);
            action.Received();
            Assert.NotNull(second);
            action.Received(2);
            stopwatch.Stop();

            Assert.True(stopwatch.Elapsed > period);
            Assert.True(stopwatch.Elapsed < (period + period));
        }
    }
}
