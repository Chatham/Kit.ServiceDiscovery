using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache.Internal;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Chatham.Kit.ServiceDiscovery.Cache.Tests
{
    [ExcludeFromCodeCoverage]
    public class CacheServiceSubscriberFixture
    {
        public ILoggerFactory LoggerFactory { get; set; }
        public IServiceSubscriber ServiceSubscriber { get; set; }
        public ICacheClient Cache { get; set; }
        public IThrottle Throttle { get; set; }

        public CacheServiceSubscriberFixture()
        {
            LoggerFactory = Substitute.For<ILoggerFactory>();
            ServiceSubscriber = Substitute.For<IServiceSubscriber>();
            Cache = Substitute.For<ICacheClient>();
            Throttle = Substitute.For<IThrottle>();
        }

        public ICacheServiceSubscriber CreateSut()
        {
            return new CacheServiceSubscriber(LoggerFactory, ServiceSubscriber, Cache, Throttle);
        }
    }
}
