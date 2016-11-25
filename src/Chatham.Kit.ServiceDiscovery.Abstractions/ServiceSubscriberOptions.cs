using System;
using System.Collections.Generic;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public class ServiceSubscriberOptions
    {
        public static readonly ServiceSubscriberOptions Default = new ServiceSubscriberOptions();

        public string DataCenter { get; set; }
        public List<string> Tags { get; set; }
        public bool PassingOnly { get; set; } = true;

        public int MaxUpdatesPerPeriod { get; set; } = 5;
        public TimeSpan MaxUpdatesPeriod { get; set; } = TimeSpan.FromSeconds(10);
    }
}
