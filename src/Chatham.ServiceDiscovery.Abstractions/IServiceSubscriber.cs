using System;
using System.Collections.Generic;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber
    {
        List<Uri> EndPoints();
    }
}
