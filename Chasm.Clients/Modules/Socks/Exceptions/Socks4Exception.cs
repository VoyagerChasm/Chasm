namespace Chasm.Clients.Modules.Socks.Exceptions
{
    public class Socks4Exception : SocksException
    {
        public Socks4Exception(string message) : base("Socks4 Error. " + message)
        {
        }
    }
}
