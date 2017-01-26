using System;

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

        public Uri ToUri(string scheme = "http", string basePath = "/")
        {
            var builder = new UriBuilder(scheme, Host, Port, basePath);
            return builder.Uri;
        }
    }
}
