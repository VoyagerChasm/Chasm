using Chasm.Clients.Modules.DnsResolver;
using System;

namespace Chasm.Clients.Modules.Socks
{
    public abstract class AuthSocks : Socks
    {
        protected bool _auth;
        protected readonly string _username;
        protected readonly string _password;

        public AuthSocks(string host, uint port, IDnsResolver resolver = null) : base(host, port, resolver)
        {
            _auth = false;
        }

        public AuthSocks(string host, uint port, string username, string password, IDnsResolver resolver = null) : this(host, port, resolver)
        {
            _auth = true;

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username must to be not null or empty");

            if (password is null)
                throw new ArgumentNullException("Password must to be not null");

            _username = username;
            _password = password;
        }
    }
}
