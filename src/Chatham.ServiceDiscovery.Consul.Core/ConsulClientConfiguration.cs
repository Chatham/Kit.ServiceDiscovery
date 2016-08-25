using System;
using System.Diagnostics.CodeAnalysis;

namespace Chatham.ServiceDiscovery.Consul.Core
{
    [ExcludeFromCodeCoverage]
    public class ConsulClientConfiguration
    {
        public Uri Address { get; set; }
        public string Token { get; set; }
    }
}
