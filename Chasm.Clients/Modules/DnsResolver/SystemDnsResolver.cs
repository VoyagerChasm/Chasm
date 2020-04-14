using System;
using System.Linq;
using System.Net;

namespace Chasm.Clients.Modules.DnsResolver
{
    public class SystemDnsResolver : IDnsResolver
    {
        public IPAddress Resolve(string hostname)
        {
            if (string.IsNullOrWhiteSpace(hostname))
                throw new ArgumentException("Host must to be not null or empty", nameof(hostname));

            if (IPAddress.TryParse(hostname, out var address))
            {
                return address;
            }

            return Dns.GetHostAddresses(hostname).FirstOrDefault();
        }

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
