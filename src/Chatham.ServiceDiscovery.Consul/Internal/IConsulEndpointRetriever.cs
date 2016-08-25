using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatham.ServiceDiscovery.Consul.Internal
{
    public interface IConsulEndpointRetriever
    {
        Task<List<Uri>> FetchEndpoints();
    }
}
