using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatham.ServiceDiscovery.Consul.Core
{
    public interface IConsulClientAdapter
    {
        Task<List<Uri>> FetchEndpoints();
    }
}
