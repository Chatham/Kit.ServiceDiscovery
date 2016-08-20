using System;
using System.Collections.Generic;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface ISubscriber
    {
        List<Uri> EndPoints();
    }
}
