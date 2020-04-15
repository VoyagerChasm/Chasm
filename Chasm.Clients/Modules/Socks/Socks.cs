using Chasm.Clients.Modules.DnsResolver;
using System.Net.Sockets;

namespace Chasm.Clients.Modules.Socks
{
    public abstract class Socks : ISocks
    {
        protected IDnsResolver _resolver;

        public Socks(IDnsResolver resolver = null)
        {
            _resolver = resolver;
        }

        public abstract void CreateTunnel(Socket socket, string destHost, uint destPort);
    }
}
