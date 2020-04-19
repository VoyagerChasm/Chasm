using Chasm.Clients.Dns.Resolver;
using Chasm.Clients.Socks;
using Chasm.Models;
using Chasm.Models.Sockets;
using System;
using System.Net.Sockets;

namespace Chasm.Clients
{
    public class TcpClientFactory 
    {

        public TcpClientFactory(SocketAddress proxyAddress = null, Credential proxyCredential = null, IDnsResolver resolver = null)
        {
            ProxyAddress = proxyAddress;
            ProxyCredential = proxyCredential;
            Resolver = resolver;
        }


        public IDnsResolver Resolver { get; set; }
        public SocketAddress ProxyAddress { get; set; }
        public Credential ProxyCredential { get; set; }
       

        /// <summary>
        /// Create a TcpClient using a TcpClientType Proxy.
        /// </summary>
        /// <param name="destination">The address destination. Where to CONNECT</param>
        /// <param name="type">The Proxy Type</param>
        /// <param name="userId">A string used with socks 4</param>
        /// <returns></returns>
        public TcpClient CreateTcpClient(SocketAddress destination, TcpClientType type, string userId = "")
        {

            if (destination == null)
                throw new ArgumentNullException(nameof(destination), $"{nameof(destination)} must be not null");

            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ProxyAddress.Host, ProxyAddress.Port);

            try
            {
                
                if (type == TcpClientType.SOCKS4)
                {
                    var socks = new Socks4(Resolver);
                    socks.CreateTunnel(socket, destination, userId);
                }
                else if (type == TcpClientType.SOCKS4a)
                {
                    var socks = new Socks4a(Resolver);
                    socks.CreateTunnel(socket, destination, userId);
                }
                else
                {
                    var socks = new Socks5(Resolver);
                    socks.CreateTunnel(socket, destination, ProxyCredential);
                }
            }
            catch (Exception e)
            {
                if (socket != null && socket.Connected)
                    socket.Close();

                throw e;
            }

            return new TcpClient() { Client = socket };
        }

    }
}
