using System.Collections.Generic;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public class ServiceSubscriberOptions
    {
        public string DataCenter { get; set; }
        public List<string> Tags { get; set; }
        public bool OnlyPassing { get; set; }
    }
}
