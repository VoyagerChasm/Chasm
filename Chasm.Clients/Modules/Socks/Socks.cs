using Chasm.Clients.Modules.DnsResolver;
using System;
using System.Net.Sockets;

namespace Chasm.Clients.Modules.Socks
{
    public abstract class Socks : ISocks
    {
        protected string _host;
        protected readonly uint _port;
        protected IDnsResolver _resolver;

        public Socks(string host, uint port, IDnsResolver resolver = null)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException(nameof(host), "Address must to be not null or empty");

            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be greater than 0 and less than 65535");

            _host = host;
            _port = port;

            _resolver = resolver;
        }

        public abstract void Connect(Socket socket);
    }
}
