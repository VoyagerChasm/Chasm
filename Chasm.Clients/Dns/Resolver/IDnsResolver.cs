using System.Net;

namespace Chasm.Clients.Dns.Resolver
{
    /// <summary>
    /// An interface used to implement a custom DnsResolver
    /// </summary>
    public interface IDnsResolver
    {
        /// <summary>
        /// Resolve hostname to IP.
        /// If an error occurs throw an Exception.
        /// </summary>
        /// <param name="hostname">The hostname to resolve</param>
        /// <returns>The IPAddress of the hostname</returns>
        IPAddress Resolve(string hostname);

        /// <summary>
        /// Try resolve the hostname to IP.
        /// If an error occurs IPAddress is <see langword="null"></see>
        /// </summary>
        /// <param name="hostname">The hostname to resolve</param>
        /// <param name="ipAddress">The IPAddress resolved</param>
        /// <returns><see langword="true"></see> if hostname is resolved, <see langword="false"></see> otherwise</returns>
        bool TryResolve(string hostname, out IPAddress ipAddress);

    }
}
