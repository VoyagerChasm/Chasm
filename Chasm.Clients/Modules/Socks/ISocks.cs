using Chasm.Clients.Modules.DnsResolver;
using System.Net.Sockets;

namespace Chasm.Clients.Modules.Socks
{
    public interface ISocks
    {

        public void CreateTunnel(Socket socket, string destHost, uint destPort);

    }
}
