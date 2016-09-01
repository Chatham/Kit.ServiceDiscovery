using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.Abstractions.Tests
{
    [ExcludeFromCodeCoverage]
    public class EndpointTests
    {
        [Fact]
        public void ToUri_WithNullScheme_ReturnsUriWithHttpScheme()
        {
            var endpoint = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 123
            };

            var actual = endpoint.ToUri();

            Assert.Equal(Uri.UriSchemeHttp, actual.Scheme);
        }

        [Fact]
        public void ToUri_WithScheme_ReturnsUriWithCorrectScheme()
        {
            var endpoint = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 123
            };

            var actual = endpoint.ToUri(Uri.UriSchemeHttps);

            Assert.Equal(Uri.UriSchemeHttps, actual.Scheme);
        }

        [Fact]
        public void ToString_WithHostAndPort_ReturnsHostAndPortString()
        {
            var endpoint = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 123
            };

            var actual = endpoint.ToString();
            Assert.Equal($"{endpoint.Host}:{endpoint.Port}", actual);
        }
    }
}
