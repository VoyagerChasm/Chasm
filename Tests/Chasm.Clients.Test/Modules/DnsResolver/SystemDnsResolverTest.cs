using Chasm.Clients.Modules.DnsResolver;
using System;
using Xunit;

namespace Chasm.Clients.Test.Modules.DnsResolver
{
    public class SystemDnsResolverTest
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TestResolveThrowException(string hostname)
        {
            Assert.ThrowsAny<Exception>(() => new SystemDnsResolver().Resolve(hostname));
        }

        [Theory]
        [InlineData("192.168.15.1")]
        [InlineData("www.google.it")]
        public void TestResolve(string hostname)
        {
            SystemDnsResolver resolver = new SystemDnsResolver();
            var result = resolver.Resolve(hostname);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("192.168.15.1")]
        [InlineData("www.google.it")]
        public void TestTryValidResolve(string hostname)
        {
            SystemDnsResolver resolver = new SystemDnsResolver();
            var result = resolver.TryResolve(hostname, out var ip);
            Assert.True(result);
            Assert.NotNull(ip);
        }

        [Theory]
        [InlineData("192.168.15.256")]
        [InlineData("www.google.")]
        public void TestTryInvalidResolve(string hostname)
        {
            SystemDnsResolver resolver = new SystemDnsResolver();
            var result = resolver.TryResolve(hostname, out var ip);
            Assert.False(result);
            Assert.Null(ip);
        }





    }
}
