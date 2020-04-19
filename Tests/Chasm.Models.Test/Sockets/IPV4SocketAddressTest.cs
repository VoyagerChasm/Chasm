using Chasm.Models.Sockets;
using System;
using Xunit;

namespace Chasm.Models.Test.Sockets
{
    public class IPV4SocketAddressTest
    {

        [Theory]
        [InlineData(null, 0)]
        public void TestConstructorThrowsArgumentNullException(string host, ushort port)
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() => new IPv4SocketAddress(host, port));
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", 0)]
        public void TestConstructorThrowsArgumentException(string host, ushort port)
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new IPv4SocketAddress(host, port));
        }

        [Theory]
        [InlineData("-1.-1.-1.-1", 0)]
        [InlineData("-1.255.255.255", 0)]
        [InlineData("255.-1.255.255", 0)]
        [InlineData("255.255.-1.255", 0)]
        [InlineData("255.255.255.-1", 0)]
        [InlineData("256.256.256.256", 0)]
        [InlineData("256.255.255.255", 0)]
        [InlineData("192.256.255.255", 0)]
        [InlineData("192.168.256.255", 0)]
        [InlineData("192.168.15.256", 0)]
        [InlineData("192.168.15...", 0)]
        [InlineData("192.168.15.222.", 0)]
        [InlineData("192.168.15..", 0)]
        [InlineData("192.168.15.", 0)]
        [InlineData("192...", 0)]
        [InlineData("192.168.", 0)]
        [InlineData("192..", 0)]
        [InlineData("192.", 0)]
        [InlineData(".", 0)]
        public void TestConstructorThrowsFormatException(string host, ushort port)
        {
            Exception ex = Assert.Throws<FormatException>(() => new IPv4SocketAddress(host, port));
        }

        [Theory]
        [InlineData("192", 0)]
        [InlineData("192.168", 0)]
        [InlineData("192.168.15", 0)]
        [InlineData("192.168.15.255", 0)]
        public void TestVaidConstructor(string host, ushort port)
        {
            IPv4SocketAddress ipv4SocketAddress = null;
            Assert.Null(ipv4SocketAddress);
            ipv4SocketAddress = new IPv4SocketAddress(host, port);
            Assert.NotNull(ipv4SocketAddress);
            Assert.IsType<IPv4SocketAddress>(ipv4SocketAddress);
        }

    }
}
