using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class ServiceSubscriberOptions
    {
        public static readonly ServiceSubscriberOptions Default = new ServiceSubscriberOptions();

        public string DataCenter { get; set; }
        public List<string> Tags { get; set; }
        public bool PassingOnly { get; set; } = true;
    }
}
