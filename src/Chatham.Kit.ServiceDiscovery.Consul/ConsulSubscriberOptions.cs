using System.Collections.Generic;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    public class ConsulSubscriberOptions
    {
        public static readonly ConsulSubscriberOptions Default = new ConsulSubscriberOptions();

        public List<string> Tags { get; set; }
        public bool PassingOnly { get; set; } = true;
    }
}
