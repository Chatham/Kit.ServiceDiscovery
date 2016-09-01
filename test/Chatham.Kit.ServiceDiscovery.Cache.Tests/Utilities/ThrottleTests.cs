using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.Cache.Tests.Utilities
{
    [ExcludeFromCodeCoverage]
    public class ThrottleTests
    {
        [Fact]
        public async Task Queue_WithSingleActionCall_FiresAction()
        {
            var period = new TimeSpan(0, 0, 2);
            var target = new Throttle(1, period);

            var action = Substitute.For<Func<string>>();
            action().Returns("hello");

            var actual = await target.Queue(action, CancellationToken.None);

            Assert.NotNull(actual);
            action.Received(1);
        }

        [Fact]
        public async Task Queue_WithTwoActionCallsAndLimitOfOnePerPeriod_FiresBothActionsBeforeTwiceTheDelay()
        {
            var period = new TimeSpan(0, 0, 2);
            var target = new Throttle(1, period);

            var action = Substitute.For<Func<string>>();
            action().Returns("hello");

            var stopwatch = Stopwatch.StartNew();
            var first = await target.Queue(action, CancellationToken.None);
            var second = await target.Queue(action, CancellationToken.None);

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
