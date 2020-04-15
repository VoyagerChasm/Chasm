using Chasm.Clients.Modules.DnsResolver;
using Chasm.Clients.Modules.Socks.Exceptions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chasm.Clients.Modules.Socks
{
    public class Socks4 : Socks
    {
        protected internal const byte SOCKS4_VERSION_NUMBER = 4;
        protected internal const byte USER_ID_TERMINATOR = 0x00;
        protected internal const byte SOCKS4_CMD_CONNECT = 0x01;

        private const byte SOCKS4_RESPONSE_VERSION = 0x00;

        private const byte SOCKS4_CMD_REPLY_REQUEST_GRANTED = 90;
        private const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_OR_FAILED = 91;
        private const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_CANNOT_CONNECT_TO_IDENTD = 92;
        private const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_DIFFERENT_IDENTD = 93;


        protected readonly string _userId;

        public Socks4(IDnsResolver resolver = null) : base(resolver)
        {
            _userId = "";

            if (resolver is null)
                _resolver = new SystemDnsResolver();
        }

        public Socks4(string userId, IDnsResolver resolver = null) : base(resolver)
        {
            _userId = userId;

            if (resolver is null)
                _resolver = new SystemDnsResolver();
        }



        public override void CreateTunnel(Socket socket, string host, uint port)
        {
            if (socket is null)
                throw new ArgumentNullException(nameof(socket), "Socket has to be not null");

            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException(nameof(host), "Address must to be not null or empty");

            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be greater than 0 and less than 65535");

            var msg = BuildRequestMessage(host, (int)port);
            socket.Send(msg);

            var response = new byte[8];
            socket.Receive(response);

            if (response is null)
                throw new ArgumentNullException(nameof(response), "Response error");

            ValidateRequestMessageResponse(response);
        }


        protected internal virtual byte[] BuildRequestMessage(string host, int port)
        {
            // PROXY SERVER REQUEST
            // The client connects to the SOCKS server and sends a CONNECT request when
            // it wants to establish a connection to an application server. The client
            // includes in the request packet the IP address and the port number of the
            // destination host, and userid, in the following format.
            //
            //        +----+----+----+----+----+----+----+----+----+----+....+----+
            //        | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL|
            //        +----+----+----+----+----+----+----+----+----+----+....+----+
            // # of bytes:	   1    1      2              4           variable       1
            //
            // VN is the SOCKS protocol version number and should be 4. CD is the
            // SOCKS command code and should be 1 for CONNECT request. NULL is a byte
            // of all zero bits.         

            //  userId needs to be a zero length string so that the GetBytes method
            //  works properly

            var destIp = GetIPAddressBytes(host);
            var destPort = GetPortByte(port);
            var userIdBytes = Encoding.ASCII.GetBytes(_userId);
            var request = new byte[9 + userIdBytes.Length];

            //  set the bits on the request byte array
            request[0] = SOCKS4_VERSION_NUMBER;
            request[1] = SOCKS4_CMD_CONNECT;
            destPort.CopyTo(request, 2);
            destIp.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);
            request[8 + userIdBytes.Length] = USER_ID_TERMINATOR;  // null (byte with all zeros) terminator for userId

            return request;
        }

        private byte[] GetIPAddressBytes(string host)
        {
            //  if the address doesn't parse then try to resolve with dns
            if (!IPAddress.TryParse(host, out var ipAddr))
                ipAddr = _resolver.Resolve(host);

            // return address bytes
            return ipAddr.GetAddressBytes();
        }

        protected internal byte[] GetPortByte(int port)
        {
            var array = new byte[2];
            array[0] = Convert.ToByte(port / 256);
            array[1] = Convert.ToByte(port % 256);
            return array;
        }

        private void ValidateRequestMessageResponse(byte[] response)
        {

            // PROXY SERVER RESPONSE
            // The SOCKS server checks to see whether such a request should be granted
            // based on any combination of source IP address, destination IP address,
            // destination port number, the userid, and information it may obtain by
            // consulting IDENT, cf. RFC 1413.  If the request is granted, the SOCKS
            // server makes a connection to the specified port of the destination host.
            // A reply packet is sent to the client when this connection is established,
            // or when the request is rejected or the operation fails. 
            //
            //              +----+----+----+----+----+----+----+----+
            //              | VN | CD | DSTPORT |      DSTIP        |
            //              +----+----+----+----+----+----+----+----+
            // # of bytes:	   1    1      2              4
            //
            // VN is the version of the reply code and should be 0. CD is the result
            // code with one of the following values:
            //
            //    90: request granted
            //    91: request rejected or failed
            //    92: request rejected becuase SOCKS server cannot connect to
            //        identd on the client
            //    93: request rejected because the client program and identd
            //        report different user-ids
            //
            // The remaining fields are ignored.
            //
            // The SOCKS server closes its connection immediately after notifying
            // the client of a failed or rejected request. For a successful request,
            // the SOCKS server gets ready to relay traffic on both directions. This
            // enables the client to do I/O on its connection as if it were directly
            // connected to the application server.

            if (response[0] != SOCKS4_RESPONSE_VERSION)
                throw new Socks4Exception("Invalid version recived. The version has to be 0");

            var replyCode = response[1];
            if (replyCode != SOCKS4_CMD_REPLY_REQUEST_GRANTED)
            {
                var proxyErrorText = replyCode switch
                {
                    SOCKS4_CMD_REPLY_REQUEST_REJECTED_OR_FAILED => "Connection request was rejected or failed",
                    SOCKS4_CMD_REPLY_REQUEST_REJECTED_CANNOT_CONNECT_TO_IDENTD => "Connection request was rejected because SOCKS destination cannot connect to identd on the client",
                    SOCKS4_CMD_REPLY_REQUEST_REJECTED_DIFFERENT_IDENTD => "Connection request rejected because the client program and identd report different user-ids",
                    _ => string.Format("Client received an unknown reply with the code value '{0}' from the destination", replyCode.ToString()),
                };

                throw new Socks4Exception(proxyErrorText);
            }
        }

    }
}
