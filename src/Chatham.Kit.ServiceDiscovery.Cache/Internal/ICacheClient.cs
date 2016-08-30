namespace Chatham.Kit.ServiceDiscovery.Cache.Internal
{
    public interface ICacheClient
    {
        T Get<T>(object key);
        T Set<T>(object key, T value);
        void Remove(object key);
    }
}
