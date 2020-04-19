using Chasm.Clients.Dns.Resolver;
using System.Text;

namespace Chasm.Clients.Socks
{

    /// <summary>
    /// Rapresent a Socks4a standard implementation.
    /// </summary>
    public class Socks4a : Socks4
    {


        /// <summary>
        /// Constructor.
        /// If no resolver is passed it use <see cref="SystemDnsResolver"/>
        /// </summary>
        /// <param name="resolver">A custom DNS Resolver</param>
        public Socks4a(IDnsResolver resolver = null) : base(resolver)
        {
        }

        protected internal override byte[] BuildRequestMessage(string host, int port, string userId)
        {
            // PROXY SERVER REQUEST
            //Please read SOCKS4.protocol first for an description of the version 4
            //protocol. This extension is intended to allow the use of SOCKS on hosts
            //which are not capable of resolving all domain names.
            //
            //In version 4, the client sends the following packet to the SOCKS server
            //to request a CONNECT or a BIND operation:
            //
            //        +----+----+----+----+----+----+----+----+----+----+....+----+
            //        | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL|
            //        +----+----+----+----+----+----+----+----+----+----+....+----+
            // # of bytes:	   1    1      2              4           variable       1
            //
            //VN is the SOCKS protocol version number and should be 4. CD is the
            //SOCKS command code and should be 1 for CONNECT or 2 for BIND. NULL
            //is a byte of all zero bits.
            //
            //For version 4A, if the client cannot resolve the destination host's
            //domain name to find its IP address, it should set the first three bytes
            //of DSTIP to NULL and the last byte to a non-zero value. (This corresponds
            //to IP address 0.0.0.x, with x nonzero. As decreed by IANA  -- The
            //Internet Assigned Numbers Authority -- such an address is inadmissible
            //as a destination IP address and thus should never occur if the client
            //can resolve the domain name.) Following the NULL byte terminating
            //USERID, the client must sends the destination domain name and termiantes
            //it with another NULL byte. This is used for both CONNECT and BIND requests.
            //
            //A server using protocol 4A must check the DSTIP in the request packet.
            //If it represent address 0.0.0.x with nonzero x, the server must read
            //in the domain name that the client sends in the packet. The server
            //should resolve the domain name and make connection to the destination
            //host if it can. 
            //
            //SOCKSified sockd may pass domain names that it cannot resolve to
            //the next-hop SOCKS server.    

            byte[] destIp = { 0, 0, 0, 1 };  // build the invalid ip address as specified in the 4a protocol
            var destPort = GetPortByte(port);
            var userIdBytes = Encoding.ASCII.GetBytes(userId);
            var hostBytes = Encoding.ASCII.GetBytes(host);



            var request = new byte[10 + userIdBytes.Length + hostBytes.Length];

            request[0] = SOCKS4_VERSION_NUMBER;
            request[1] = SOCKS4_CMD_CONNECT;
            destPort.CopyTo(request, 2);
            destIp.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);  // copy the userid to the request byte array
            request[8 + userIdBytes.Length] = USER_ID_TERMINATOR;  // null (byte with all zeros) terminator for userId
            hostBytes.CopyTo(request, 9 + userIdBytes.Length);  // copy the host name to the request byte array
            request[9 + userIdBytes.Length + hostBytes.Length] = USER_ID_TERMINATOR;  // null (byte with all zeros) terminator for userId

            return request;
        }
    }
}
