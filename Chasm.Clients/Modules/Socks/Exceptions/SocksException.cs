using System;

namespace Chasm.Clients.Modules.Socks.Exceptions
{
    public class SocksException : Exception
    {
        public SocksException(string message) : base(message)
        {
        }
    }
}
