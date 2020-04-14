using System.Net.Sockets;

namespace Chasm.Clients.Modules.Socks
{
    public interface ISocks
    {

        public void Connect(Socket socket);

    }
}
