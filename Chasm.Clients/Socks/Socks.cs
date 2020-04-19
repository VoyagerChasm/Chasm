using Chasm.Clients.Dns.Resolver;

namespace Chasm.Clients.Socks
{
    public abstract class Socks : ISocks
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resolver">A custom DNS Resolver</param>
        public Socks(IDnsResolver resolver = null)
        {
            Resolver = resolver;
        }

        public IDnsResolver Resolver { get; set; }
    }
}
