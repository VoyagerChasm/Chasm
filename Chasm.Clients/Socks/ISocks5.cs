using Chasm.Models;
using Chasm.Models.Sockets;
using System.Net.Sockets;

namespace Chasm.Clients.Socks
{
    public interface ISocks5 : ISocks
    {

        /// <summary>
        /// Create a pass through connection to the specified destination host
        /// </summary>
        /// <param name="socket">The socket connected to the Socks Server</param>
        /// <param name="destinationAddress">The destination host and port where the Socks Server have to Connect</param>
        /// <param name="proxyCredentials">Userame and Password for proxy server</param>
        /// <remarks>
        /// This method creates a connection to the proxy server and instructs the proxy server
        /// to make a pass through connection to the specified destination host on the specified
        /// port.  
        /// </remarks>
        public void CreateTunnel(Socket socket, SocketAddress destinationAddress, Credential proxyCredentials = null);

    }
}
