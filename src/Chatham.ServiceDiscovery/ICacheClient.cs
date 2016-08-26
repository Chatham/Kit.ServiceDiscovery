namespace Chatham.ServiceDiscovery
{
    public interface ICacheClient
    {
        T Get<T>(object key);
        T Set<T>(object key, T value);
        void Remove(object key);
    }
}
