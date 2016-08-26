using System.Collections.Generic;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public class ServiceSubscriberOptions
    {
        public static readonly ServiceSubscriberOptions Default = new ServiceSubscriberOptions();

        public string DataCenter { get; set; }
        public List<string> Tags { get; set; }
        public bool PassingOnly { get; set; }
    }
}
