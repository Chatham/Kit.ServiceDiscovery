using System.Threading;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache.Utilities;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Chatham.Kit.ServiceDiscovery.Cache.Tests
{
    public class CacheServiceSubscriberFixture
    {
        public ILogger Logger { get; set; }
        public IServiceSubscriber ServiceSubscriber { get; set; }
        public ICacheClient Cache { get; set; }
        public IThrottle Throttle { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public CacheServiceSubscriberFixture()
        {
            Logger = Substitute.For<ILogger>();
            ServiceSubscriber = Substitute.For<IServiceSubscriber>();
            Cache = Substitute.For<ICacheClient>();
            Throttle = Substitute.For<IThrottle>();

            CancellationTokenSource = new CancellationTokenSource();
        }

        public IServiceSubscriber CreateSut()
        {
            return new CacheServiceSubscriber(Logger, ServiceSubscriber, Cache, Throttle, CancellationTokenSource);
        }
    }
}
