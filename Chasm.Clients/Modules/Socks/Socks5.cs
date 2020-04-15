using Chasm.Clients.Modules.DnsResolver;
using Chasm.Clients.Modules.Socks.Exceptions;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chasm.Clients.Modules.Socks
{
    public class Socks5 : AuthSocks
    {

        private const byte SOCKS5_VERSION_NUMBER = 0x05;

        private const byte SOCKS5_AUTH_METHODS_NOT_SUPPORTED = 1;
        private const byte SOCKS5_AUTH_METHODS_SUPPORTED = 2;

        private const byte SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED = 0x00;
        private const byte SOCKS5_AUTH_METHOD_GSSAPI = 0x01;
        private const byte SOCKS5_AUTH_METHOD_USERNAME_PASSWORD = 0x02;
        private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_BEGIN = 0x03;
        //private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_END = 0x7f;
        //private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_BEGIN = 0x80;
        private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_END = 0xfe;
        private const byte SOCKS5_AUTH_METHOD_NO_ACCEPTABLE_METHODS = 0xff;

        private const byte SOCKS5_SUBNEGOTIATION_VERSION = 0x01;

        private const byte SOCKS5_ADDRTYPE_IPV4 = 0x01;
        private const byte SOCKS5_ADDRTYPE_DOMAIN_NAME = 0x03;
        private const byte SOCKS5_ADDRTYPE_IPV6 = 0x04;

        private const byte SOCKS5_RESERVED = 0x00;

        private const byte SOCKS5_CMD_CONNECT = 0x01;

        private const byte SOCKS5_CMD_REPLY_SUCCEEDED = 0x00;
        private const byte SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE = 0x01;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET = 0x02;
        private const byte SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE = 0x03;
        private const byte SOCKS5_CMD_REPLY_HOST_UNREACHABLE = 0x04;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_REFUSED = 0x05;
        private const byte SOCKS5_CMD_REPLY_TTL_EXPIRED = 0x06;
        private const byte SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED = 0x07;
        private const byte SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED = 0x08;

        public Socks5(IDnsResolver resolver = null) : base(resolver)
        {
        }

        public Socks5(string username, string password, IDnsResolver resolver = null) : base(username, password, resolver)
        {
        }


        public override void CreateTunnel(Socket socket, string host, uint port)
        {
            if (socket is null)
                throw new ArgumentNullException(nameof(socket), "Socket has to be not null");

            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException(nameof(host), "Address must to be not null or empty");

            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be greater than 0 and less than 65535");

            var ngAuthMsg = BuildNeogtiateAuthenticationMessage(_auth);
            socket.Send(ngAuthMsg);

            var ngAuthMsgByte = new byte[2];

            if (socket.Receive(ngAuthMsgByte) != 2)
                throw new Socks5Exception("Invalid response size. Negotiation message response for authentication must be 2 byte.");

            VaidateNeogtiateAuthenticationMessageRespone(ngAuthMsgByte, _auth);

            if (_auth)
            {
                var authMsg = BuildAuthenticationMessage();
                socket.Send(authMsg);

                var authMsgByte = new byte[2];

                if (socket.Receive(authMsgByte) != 2)
                    throw new Socks5Exception("Invalid response size. Authentication message response must be 2 byte.");

                VaidateAuthenticationMessageRespone(authMsgByte);
            }

            var addressType = GetAddressType(host);
            if (addressType == SOCKS5_ADDRTYPE_DOMAIN_NAME && _resolver != null)
                if (_resolver.TryResolve(host, out var ipAddress))
                    host = ipAddress.ToString();
                else
                    throw new Socks5Exception("Destination address unreachable.");

            var connectCmdMsg = BuildRequestMessage(SOCKS5_CMD_CONNECT, addressType, host, (int)port);
            socket.Send(connectCmdMsg);

            var connectCmdMsgByte = new byte[255];
            var connectCmdMsgRecByte = socket.Receive(connectCmdMsgByte);

            ValidateRequestMessageResponse(connectCmdMsgRecByte, connectCmdMsgByte);
        }


        private byte[] BuildNeogtiateAuthenticationMessage(bool authenticate)
        {
            //      +----+----------+----------+
            //      |VER | NMETHODS | METHODS  |
            //      +----+----------+----------+
            //      | 1  |    1     | 1 to 255 |
            //      +----+----------+----------+

            var authRequest = new byte[authenticate ? 4 : 3];
            authRequest[0] = SOCKS5_VERSION_NUMBER;
            authRequest[1] = authenticate ? SOCKS5_AUTH_METHODS_SUPPORTED : SOCKS5_AUTH_METHODS_NOT_SUPPORTED;
            authRequest[2] = SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED;
            if (authenticate)
                authRequest[3] = SOCKS5_AUTH_METHOD_USERNAME_PASSWORD;

            return authRequest;
        }

        private void VaidateNeogtiateAuthenticationMessageRespone(byte[] buffer, bool authenticate)
        {

            //     +----+--------+
            //     |VER | METHOD |
            //     +----+--------+
            //     | 1  |   1    |
            //     +----+--------+
            //
            //  If the selected METHOD is X'FF', none of the methods listed by the
            //  client are acceptable, and the client MUST close the connection.
            //
            //  The values currently defined for METHOD are:
            //   * X'00' NO AUTHENTICATION REQUIRED
            //   * X'01' GSSAPI
            //   * X'02' USERNAME/PASSWORD
            //   * X'03' to X'7F' IANA ASSIGNED
            //   * X'80' to X'FE' RESERVED FOR PRIVATE METHODS
            //   * X'FF' NO ACCEPTABLE METHODS

            if (buffer[0] != SOCKS5_VERSION_NUMBER)
                throw new Socks5Exception("Invalid version recived. Current version is 5");

            if (buffer[1] == SOCKS5_AUTH_METHOD_NO_ACCEPTABLE_METHODS)
                throw new Socks5Exception("The destination does not accept the supported client authentication methods.");

            if (buffer[1] == SOCKS5_AUTH_METHOD_GSSAPI || buffer[1] >= SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_BEGIN && buffer[1] <= SOCKS5_AUTH_METHOD_RESERVED_RANGE_END)
                throw new SocksException("Client Error. Authentication not currently supported.");

            if (buffer[1] == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD && !authenticate)
                throw new Socks5Exception("Authentication requtested.");

            if (buffer[1] == SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED && authenticate)
                throw new Socks5Exception("No authentication requested.");
        }

        private byte[] BuildAuthenticationMessage()
        {
            // USERNAME / PASSWORD SERVER REQUEST
            // Once the SOCKS V5 server has started, and the client has selected the
            // Username/Password Authentication protocol, the Username/Password
            // subnegotiation begins.  This begins with the client producing a
            // Username/Password request:
            //
            //       +----+------+----------+------+----------+
            //       |VER | ULEN |  UNAME   | PLEN |  PASSWD  |
            //       +----+------+----------+------+----------+
            //       | 1  |  1   | 1 to 255 |  1   | 1 to 255 |
            //       +----+------+----------+------+----------+

            // create a data structure (binary array) containing credentials
            // to send to the proxy server which consists of clear username and password data

            var usernameBytes = Encoding.UTF8.GetBytes(_username);
            if (usernameBytes.Length > 255)
                throw new ArgumentOutOfRangeException("Username is too long");

            var passwordBytes = Encoding.UTF8.GetBytes(_password);
            if (passwordBytes.Length > 255)
                throw new ArgumentOutOfRangeException("Password is too long");

            var authMessage = new byte[3 + usernameBytes.Length + passwordBytes.Length];
            authMessage[0] = SOCKS5_SUBNEGOTIATION_VERSION;
            authMessage[1] = (byte)usernameBytes.Length;
            Array.Copy(usernameBytes, 0, authMessage, 2, usernameBytes.Length);
            authMessage[2 + usernameBytes.Length] = (byte)passwordBytes.Length;
            Array.Copy(passwordBytes, 0, authMessage, 3 + usernameBytes.Length, passwordBytes.Length);

            return authMessage;
        }

        private void VaidateAuthenticationMessageRespone(byte[] buffer)
        {

            // USERNAME / PASSWORD SERVER RESPONSE
            // The server verifies the supplied UNAME and PASSWD, and sends the
            // following response:
            //
            //   +----+--------+
            //   |VER | STATUS |
            //   +----+--------+
            //   | 1  |   1    |
            //   +----+--------+
            //
            // A STATUS field of X'00' indicates success. If the server returns a
            // `failure' (STATUS value other than X'00') status, it MUST close the
            // connection.

            if (buffer[0] != SOCKS5_SUBNEGOTIATION_VERSION)
                throw new Socks5Exception("Invalid subnegotiation version recived.");

            if (buffer[1] != 0)
                throw new Socks5Exception("Username or password is not valid.");
        }

        private byte GetAddressType(string _host)
        {
            var result = IPAddress.TryParse(_host, out IPAddress ipAddr);

            if (!result)
                return SOCKS5_ADDRTYPE_DOMAIN_NAME;

            return ipAddr.AddressFamily switch
            {
                AddressFamily.InterNetwork => SOCKS5_ADDRTYPE_IPV4,
                AddressFamily.InterNetworkV6 => SOCKS5_ADDRTYPE_IPV6,
                _ => throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The host addess {0} of type '{1}' is not a supported address type. The supported types are InterNetwork and InterNetworkV6.", _host, Enum.GetName(typeof(AddressFamily), ipAddr.AddressFamily))),
            };
        }

        private byte[] BuildRequestMessage(byte command, byte addressType, string _host, int _port)
        {
            var addressBytes = GetAddressBytes(addressType, _host);
            var addressLength = addressBytes.Length;

            //  The connection request is made up of 6 bytes plus the
            //  length of the variable address byte array
            //
            //  +----+-----+-------+------+----------+----------+
            //  |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
            //  +----+-----+-------+------+----------+----------+
            //  | 1  |  1  | X'00' |  1   | Variable |    2     |
            //  +----+-----+-------+------+----------+----------+
            //
            // * VER protocol version: X'05'
            // * CMD
            //   * CONNECT X'01'
            //   * BIND X'02'
            //   * UDP ASSOCIATE X'03'
            // * RSV RESERVED
            // * ATYP address itemType of following address
            //   * IP V4 address: X'01'
            //   * DOMAINNAME: X'03'
            //   * IP V6 address: X'04'
            // * DST.ADDR desired destination address
            // * DST.PORT desired destination port in network octet order        

            var request = new byte[6 + addressLength];
            request[0] = SOCKS5_VERSION_NUMBER;

            request[1] = command;
            //request[2] = SOCKS5_RESERVED;
            request[3] = addressType;

            Array.Copy(addressBytes, 0, request, 4, addressLength);
            request[^2] = (byte)(_port / 256);
            request[^1] = (byte)(_port % 256);

            return request;
        }

        private byte[] GetAddressBytes(byte addressType, string _host)
        {
            switch (addressType)
            {
                case SOCKS5_ADDRTYPE_IPV4:
                case SOCKS5_ADDRTYPE_IPV6:
                    return IPAddress.Parse(_host).GetAddressBytes();

                case SOCKS5_ADDRTYPE_DOMAIN_NAME:
                    byte[] domainBytes = Encoding.UTF8.GetBytes(_host);

                    //  create a byte array to hold the host name bytes plus one byte to store the length
                    byte[] addressBytes = new byte[1 + domainBytes.Length];

                    //  if the address field contains a fully-qualified domain name.  The first
                    //  octet of the address field contains the number of octets of name that
                    //  follow, there is no terminating NUL octet.
                    addressBytes[0] = Convert.ToByte(_host.Length);
                    Array.Copy(domainBytes, 0, addressBytes, 1, domainBytes.Length);

                    return addressBytes;

                default:
                    throw new ArgumentException("Unknown address type. Currently supported address type are IPv4, IPv6 and Domain Address.", nameof(addressType));
            }
        }

        private void ValidateRequestMessageResponse(int byteRecived, byte[] response)
        {
            //  PROXY SERVER RESPONSE
            //  +----+-----+-------+------+----------+----------+
            //  |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
            //  +----+-----+-------+------+----------+----------+
            //  | 1  |  1  | X'00' |  1   | Variable |    2     |
            //  +----+-----+-------+------+----------+----------+
            //
            // * VER protocol version: X'05'
            // * REP Reply field:
            //   * X'00' succeeded
            //   * X'01' general SOCKS server failure
            //   * X'02' connection not allowed by ruleset
            //   * X'03' Network unreachable
            //   * X'04' Host unreachable
            //   * X'05' Connection refused
            //   * X'06' TTL expired
            //   * X'07' Command not supported
            //   * X'08' Address itemType not supported
            //   * X'09' to X'FF' unassigned
            // RSV RESERVED
            // ATYP address itemType of following address


            if (byteRecived < 8)
                throw new Socks5Exception("Invalid response size. Recived byte must be more than 8 byte");

            if (response[0] != SOCKS5_VERSION_NUMBER)
                throw new Socks5Exception("Invalid version recived. Current version is 5");

            var REPCode = response[1];

            if (REPCode > 8)
                throw new Socks5Exception("Invalid response size. The response code must be less than 8 byte");

            if (response[1] != SOCKS5_CMD_REPLY_SUCCEEDED)
            {
                var errorText = REPCode switch
                {
                    SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE => "A general socks destination failure occurred",
                    SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET => "The connection is not allowed by proxy destination rule set",
                    SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE => "The network was unreachable",
                    SOCKS5_CMD_REPLY_HOST_UNREACHABLE => "The host was unreachable",
                    SOCKS5_CMD_REPLY_CONNECTION_REFUSED => "The connection was refused by the remote network",
                    SOCKS5_CMD_REPLY_TTL_EXPIRED => "The time to live (TTL) has expired",
                    SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED => "The command issued by the proxy client is not supported by the proxy destination",
                    SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED => "The address type specified is not supported",
                    _ => string.Format(CultureInfo.InvariantCulture, "An unknown SOCKS reply with the code value '{0}' was received", REPCode.ToString(CultureInfo.InvariantCulture)),
                };

                throw new SocksException(errorText);
            }

            if (response[2] != SOCKS5_RESERVED)
                throw new Socks5Exception("Connection reserved.");

            var addrType = response[3];
            if (addrType != SOCKS5_ADDRTYPE_IPV4 && addrType != SOCKS5_ADDRTYPE_DOMAIN_NAME && addrType != SOCKS5_ADDRTYPE_IPV6)
                throw new Socks5Exception("Invalid address type. Currently supported address type are IPv4, IPv6 and Domain Address.");

            if (addrType == SOCKS5_ADDRTYPE_IPV4)
                if (byteRecived != 10)
                    throw new Socks5Exception("Invalid Ipv4 response size.");
                else if (addrType == SOCKS5_ADDRTYPE_IPV6)
                    if (byteRecived != 22)
                        throw new Socks5Exception("Invalid Ipv6 response size.");
                    else if (byteRecived != 7 + response[4])
                        throw new Socks5Exception("Invalid Domain Address response size.");
        }


    }
}
