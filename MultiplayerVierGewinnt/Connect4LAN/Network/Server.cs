using System;
using System.Collections.Generic;
using System.IO;
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
    class Server
    {
        private TcpListener socket;
        private Player[] players;

        /// <summary>
        /// Opens a TCP Listner Port on the named port
        /// </summary>
        /// <param name="port">The Port on wich the listning shall be</param>
        public Server(int port = 16569)
        {
            //insantiate the Socket
            socket = new TcpListener(Dns.GetHostAddresses("localhost")[0], port);
            //only 2 players can play simultaniously
            players = new Player[2];
            //start accepting incoming requests and only allow 1 person to be in queue at a time
            socket.Start(1);
        }

        /// <summary>
        /// Processes Incoming Requests
        /// </summary>
        private async void processIncomingRequests()
        {
            //get the first client
            var client = await socket.AcceptTcpClientAsync();
            players[0] = parseRequest(await (new StreamReader(client.GetStream())).ReadLineAsync());
        }

        private Player parseRequest(string v)
        {
            throw new NotImplementedException();
        }





        /// <summary>
        /// Stops the Server
        /// </summary>
        public void Stop()
        {
            socket.Stop();
        }
    }
}
