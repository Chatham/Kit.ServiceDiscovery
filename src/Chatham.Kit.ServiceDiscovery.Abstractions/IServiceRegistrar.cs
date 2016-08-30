using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public interface IServiceRegistrar
    {
        Task Register();
        Task Deregister();
    }
}
