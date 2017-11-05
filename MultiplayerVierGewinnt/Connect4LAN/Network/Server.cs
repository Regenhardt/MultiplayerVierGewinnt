using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

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
            //socket.Start(1);

			//process the incoming requests
			processIncomingRequestsASync();
        }

        /// <summary>
        /// Processes Incoming Requests
        /// </summary>
        private async void processIncomingRequestsASync()
        {
			socket.Start();
            //get the first client
            var client = await socket.AcceptTcpClientAsync();
			players[0] = parseRequest(await (new StreamReader(client.GetStream())).ReadLineAsync(), Colors.Green, client);
			//TODO JSON FOrmat
			players[0].SendMessage("Waiting for one more player...");
			client = await socket.AcceptTcpClientAsync();
			players[1] = parseRequest(await (new StreamReader(client.GetStream())).ReadLineAsync(), Colors.Red, client);

			//initlize the game
			ConnectFourGame game = new ConnectFourGame(players[0], players[1]);
			
		}

        private Player parseRequest(string json, Color color, TcpClient client)
        {
			//initilize the serilizer
			JavaScriptSerializer serializer = new JavaScriptSerializer();

			//pray to god that his works :D
			string name = serializer.Deserialize<dynamic>(json)["Name"];

			//check if name is taken
			if (players.Any(p => p != null && string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
				name += "_2";

			//instantiate the new player
			var player = new Player(color, name, null, client); //TODO: IP
			//Tell him his color and Name
			player.SendMessage(serializer.Serialize(new { Color = color.ToString(), Name = name }));

			//return him
			return player;
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
