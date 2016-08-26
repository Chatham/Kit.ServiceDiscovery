
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chatham.ServiceDiscovery.Consul.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Chatham.ServiceDiscovery.Consul.Tests.Utilities
{
    [TestClass]
    public class ThrottleTests
    {
        [TestMethod]
        public async Task Queue_WithSingleActionCall_FiresAction()
        {
            var period = new TimeSpan(0, 0, 2);
            var target = new Throttle(1, period);

            var action = Substitute.For<Func<string>>();
            action().Returns("hello");

            var actual = await target.Queue(action, CancellationToken.None);

            Assert.IsNotNull(actual);
            action.Received(1);
        }

        [TestMethod]
        public async Task Queue_WithTwoActionCallsAndLimitOfOnePerPeriod_FiresBothActionsBeforeTwiceTheDelay()
        {
            var period = new TimeSpan(0, 0, 2);
            var target = new Throttle(1, period);

            var action = Substitute.For<Func<string>>();
            action().Returns("hello");

            var stopwatch = Stopwatch.StartNew();
            var first = await target.Queue(action, CancellationToken.None);
            var second = await target.Queue(action, CancellationToken.None);

            Assert.IsNotNull(first);
            action.Received();
            Assert.IsNotNull(second);
            action.Received(2);
            stopwatch.Stop();
            
            Assert.IsTrue(stopwatch.Elapsed > period);
            Assert.IsTrue(stopwatch.Elapsed < (period + period));
        }
    }
}
