using System.Collections.Generic;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public class SubscriberOptions
    {
        public string DataCenter { get; set; }
        public List<string> Tags { get; set; }
        public bool OnlyPassing { get; set; }
    }
}
