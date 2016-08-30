using System;
using System.Diagnostics.CodeAnalysis;

namespace Chatham.Kit.ServiceDiscovery.Consul
{
    [ExcludeFromCodeCoverage]
    public class ConsulConfiguration
    {
        public Uri Address { get; set; }
        public string Token { get; set; }
    }
}
