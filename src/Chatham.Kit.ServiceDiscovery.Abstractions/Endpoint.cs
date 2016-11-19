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

        public Uri ToUri(string scheme = null)
        {
            if (scheme == null)
            {
                scheme = "http";
            }

            var builder = new UriBuilder(scheme, Host, Port);
            return builder.Uri;
        }
    }
}
