namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public class ServiceEndpoint
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}
