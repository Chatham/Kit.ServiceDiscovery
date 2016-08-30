namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public class Endpoint
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}
