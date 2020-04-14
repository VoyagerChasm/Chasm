﻿namespace Chasm.Clients.Modules.Socks.Exceptions
{
    public class Socks5Exception : SocksException
    {
        public Socks5Exception(string message) : base("Socks5 Error. " + message)
        {

        }
    }
}
