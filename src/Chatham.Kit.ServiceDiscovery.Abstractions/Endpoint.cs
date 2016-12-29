using System;

namespace Chatham.Kit.ServiceDiscovery.Abstractions
{
    public class Endpoint
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Scheme { get; set; } = "http";

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }

        public Uri ToUri()
        {
            var builder = new UriBuilder(Scheme, Host, Port);
            return builder.Uri;
        }
    }
}
