using Chasm.Models.Sockets;
using System;
using System.Net.Sockets;
using Xunit;

namespace Chasm.Clients.Test
{
    public class TcpClientFactoryTest
    {

        [Fact]
        public void CreateTcpClientTest()
        {

            var factory = new TcpClientFactory();
            Assert.Throws<ArgumentNullException>(() => factory.CreateTcpClient(null, TcpClientType.SOCKS4));

            factory.ProxyAddress = new SocketAddress("127.0.0.1", 80);

            Assert.ThrowsAny<SocketException>(() => factory.CreateTcpClient(new SocketAddress("www.gooogle.it", 80), TcpClientType.SOCKS4));
            Assert.ThrowsAny<SocketException>(() => factory.CreateTcpClient(new SocketAddress("www.gooogle.it", 80), TcpClientType.SOCKS4a));
            Assert.ThrowsAny<SocketException>(() => factory.CreateTcpClient(new SocketAddress("www.gooogle.it", 80), TcpClientType.SOCKS5));

            Assert.ThrowsAny<ArgumentNullException>(() => factory.CreateTcpClient(null, TcpClientType.SOCKS4));
            Assert.ThrowsAny<ArgumentNullException>(() => factory.CreateTcpClient(null, TcpClientType.SOCKS4a));
            Assert.ThrowsAny<ArgumentNullException>(() => factory.CreateTcpClient(null, TcpClientType.SOCKS5));


            /// TO DECOMMENT FOR TESTING ///

            //factory.ProxyAddress = new SocketAddress("address", port);

            //Assert.ThrowsAny<SocksException>(() => factory.CreateTcpClient(new SocketAddress("host", port), TcpClientType.SOCKS4));
            //Assert.ThrowsAny<SocksException>(() => factory.CreateTcpClient(new SocketAddress("host", port), TcpClientType.SOCKS4a));
            //Assert.ThrowsAny<SocksException>(() => factory.CreateTcpClient(new SocketAddress("host", port), TcpClientType.SOCKS5));

            //factory.CreateTcpClient(new SocketAddress("tcpaddress", port), TcpClientType.SOCKS5);

            //factory.ProxyAddress = new SocketAddress("address", port);
            //factory.CreateTcpClient(new SocketAddress("tcpaddress", port), TcpClientType.SOCKS4);
        }

    }
}
