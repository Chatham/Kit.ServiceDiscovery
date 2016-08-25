using System;

namespace Chatham.ServiceDiscovery.Consul.Core
{
    public class ConsulClientConfiguration
    {
        public Uri Address { get; set; }
        public string Token { get; set; }
    }
}
