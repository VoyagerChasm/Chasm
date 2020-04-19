using System;

namespace Chasm.Clients.Socks.Exceptions
{
    public class SocksException : Exception
    {
        public SocksException(string message) : base(message)
        {
        }
    }
}
