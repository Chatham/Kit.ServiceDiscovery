using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chatham.Kit.ServiceDiscovery.Abstractions.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EndpointTests
    {
        [TestMethod]
        public void ToUri_WithNullScheme_ReturnsUriWithHttpScheme()
        {
            var endpoint = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 123
            };

            var actual = endpoint.ToUri();

            Assert.AreEqual(Uri.UriSchemeHttp, actual.Scheme);
        }

        [TestMethod]
        public void ToUri_WithScheme_ReturnsUriWithCorrectScheme()
        {
            var endpoint = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 123
            };

            var actual = endpoint.ToUri(Uri.UriSchemeHttps);

            Assert.AreEqual(Uri.UriSchemeHttps, actual.Scheme);
        }

        [TestMethod]
        public void ToString_WithHostAndPort_ReturnsHostAndPortString()
        {
            var endpoint = new Endpoint
            {
                Host = Guid.NewGuid().ToString(),
                Port = 123
            };

            var actual = endpoint.ToString();
            Assert.AreEqual($"{endpoint.Host}:{endpoint.Port}", actual);
        }
    }
}
