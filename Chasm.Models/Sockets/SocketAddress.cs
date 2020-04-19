using System;

namespace Chasm.Models.Sockets
{

    /// <summary>
    /// Map a Host:Port Socket Address
    /// </summary>
    public class SocketAddress
    {

        public string Host { get; private set; }
        public ushort Port { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">The host of the socket address </param>
        /// <param name="port">The port of the socket address </param>
        public SocketAddress(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host), $"{nameof(host)} must be not null");

            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException($"{nameof(host)} is not a valid host");

            if (port < 0 || port > 0xFFFF)
                throw new ArgumentOutOfRangeException(nameof(port), $"{nameof(port)} is not a valid port address");

            Host = host;
            Port = (ushort)port;
        }

        public override bool Equals(object obj)
        {
            return obj is SocketAddress address &&
                   Host == address.Host &&
                   Port == address.Port;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Host, Port);
        }

        /// <summary>
        /// A string representation of the socket address
        /// </summary>
        /// <returns>Host:Port string representation</returns>
        public override string ToString()
        {
            return $"{Host}:{Port}";
        }

    }
}
