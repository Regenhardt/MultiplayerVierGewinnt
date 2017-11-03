using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
    /// <summary>
    /// The Server wich hosts the Game
    /// </summary>
    class Host
    {
        private TcpListener socket;

        /// <summary>
        /// Opens a TCP Listner Port on the named port
        /// </summary>
        /// <param name="port">The Port on wich the listning shall be</param>
        public Host(int port = 16569)
        {
            //insantiate the Socket
            socket = new TcpListener(Dns.GetHostAddresses("localhost")[0], port) ;
        }

        /// <summary>
        /// Processes Incoming Requests
        /// </summary>
        private void processIncomingRequests()
        {
            
        }

        /// <summary>
        /// Stops the Server
        /// </summary>
        public void Stop()
        {

        }
    }
}
