using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache;
using Chatham.Kit.ServiceDiscovery.Consul;
using Chatham.Kit.ServiceDiscovery.Throttle;

namespace ConsulServiceDiscoverySample
{
    public interface IServiceSubscriberFactory
    {
        IPollingServiceSubscriber CreateSubscriber(string serviceName);
        IPollingServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions,
            ThrottleSubscriberOptions throttleOptions);
    }

    public class ServiceSubscriberFactory : IServiceSubscriberFactory
    {
        private readonly IConsulServiceSubscriberFactory _consulServiceSubscriberFactory;
        private readonly ICacheServiceSubscriberFactory _cacheServiceSubscriberFactory;
   
        public ServiceSubscriberFactory(IConsulServiceSubscriberFactory consulServiceSubscriberFactory, ICacheServiceSubscriberFactory cacheServiceSubscriberFactory)
        {
            _consulServiceSubscriberFactory = consulServiceSubscriberFactory;
            _cacheServiceSubscriberFactory = cacheServiceSubscriberFactory;
        }

        public IPollingServiceSubscriber CreateSubscriber(string servicName)
        {
            return CreateSubscriber(servicName, ConsulSubscriberOptions.Default, ThrottleSubscriberOptions.Default);
        }

        public IPollingServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, ThrottleSubscriberOptions throttleOptions)
        {
            var consulSubscriber = _consulServiceSubscriberFactory.CreateSubscriber(serviceName, consulOptions, true);
            var throttleSubscriber = new ThrottleServiceSubscriber(consulSubscriber, throttleOptions.MaxUpdatesPerPeriod, throttleOptions.MaxUpdatesPeriod);
            return _cacheServiceSubscriberFactory.CreateSubscriber(throttleSubscriber);
        }
    }
}
