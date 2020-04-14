using System.Net;

namespace Chasm.Clients.Modules.DnsResolver
{
    public interface IDnsResolver
    {
        IPAddress Resolve(string hostname);
        bool TryResolve(string hostname, out IPAddress ipAddress);

    }
}
