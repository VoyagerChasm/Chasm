using System;
using System.Linq;
using System.Net;

namespace Chasm.Clients.Dns.Resolver
{
    /// <summary>
    /// A Dns Resolver that use the default System Dns
    /// </summary>
    public class SystemDnsResolver : IDnsResolver
    {
        /// <summary>
        /// <see cref="IDnsResolver"></see>
        /// </summary>
        /// <param name="hostname">The hostname to resolve</param>
        /// <returns>The IPAddress of the hostname</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        public IPAddress Resolve(string hostname)
        {
            if (string.IsNullOrWhiteSpace(hostname))
                throw new ArgumentException("Host must to be not null or empty", nameof(hostname));

            if (IPAddress.TryParse(hostname, out var address))
            {
                return address;
            }

            return System.Net.Dns.GetHostAddresses(hostname).FirstOrDefault();
        }

        /// <summary>
        /// <see cref="IDnsResolver"/>
        /// </summary>
        /// <param name="hostname">The hostname to resolve</param>
        /// <param name="ipAddress">The IPAddress resolved</param>
        /// <returns><see langword="true"></see> if hostname is resolved, <see langword="false"></see> otherwise</returns>
        public bool TryResolve(string hostname, out IPAddress ipAddress)
        {

            try
            {
                ipAddress = Resolve(hostname);
                return true;
            }
            catch
            {
                ipAddress = default;
                return false;
            }
        }
    }
}
