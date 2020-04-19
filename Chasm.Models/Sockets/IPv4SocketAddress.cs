using System;
using System.Net;

namespace Chasm.Models.Sockets
{

    /// <summary>
    /// Map a IPv4 Socket Address
    /// </summary>
    public class IPv4SocketAddress : SocketAddress
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">The IPv4 Address of the Socket Address </param>
        /// <param name="port">The IPv4 Port of the Socket Address </param>
        public IPv4SocketAddress(string host, ushort port) 
            : base(host, port)
        {
            if (IPAddress.Parse(host).AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                throw new ArgumentException($"{nameof(host)} is not not a valid IPv4 address");
        }
    }
}
