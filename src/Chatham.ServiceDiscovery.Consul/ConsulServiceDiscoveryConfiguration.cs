using System;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulServiceDiscoveryConfiguration
    {
        public Uri Address { get; set; }
        public string Datacenter { get; set; }
        public string Token { get; set; }
    }
}
