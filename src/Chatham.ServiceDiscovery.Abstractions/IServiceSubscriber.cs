using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatham.ServiceDiscovery.Abstractions
{
    public interface IServiceSubscriber
    {
        Task<List<Uri>> EndPointsAsync();
        List<Uri> EndPoints();
    }
}
