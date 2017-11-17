using Connect4LAN.Game;
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

namespace Connect4LAN.Network.Serverside
{
    /// <summary>
    /// The Server wich hosts the Game
    /// </summary>
    class Server
    {
        private TcpListener socket;
        private Player[] players;

        /// <summary>
        /// Opens a TCP Listner Port on the specified port 
		/// --> Starts the god damned server
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

			//process the incoming requests
			processIncomingRequestsASync();
        }


		/// <summary>
		/// Throws an exception, cuz game is already hosted
		/// </summary>///
		/// <exception cref="Exception">Always</exception>
		public void Host()
		{
			throw new Exception("Already Done nigga!");
		}

        /// <summary>
        /// Processes Incoming Requests
        /// </summary>
        private async void processIncomingRequestsASync()
        {			
            //get the first client
            var client = await socket.AcceptTcpClientAsync();
			players[0] = parseRequest(await (new StreamReader(client.GetStream())).ReadLineAsync(), Colors.Yellow, client);
			//TODO JSON FOrmat
			players[0].NetworkAdapter.SendMessage("Waiting for one more player...", NetworkMessageType.ServerMessage);
			client = await socket.AcceptTcpClientAsync();
			players[1] = parseRequest(await (new StreamReader(client.GetStream())).ReadLineAsync(), Colors.Red, client);

			//tell them about eachother
			players[0].NetworkAdapter.SendMessage(new Opponent { Color = players[1].Color, Name = players[1].Name }, NetworkMessageType.PlayerConnected);
			players[1].NetworkAdapter.SendMessage(new Opponent { Color = players[0].Color, Name = players[0].Name }, NetworkMessageType.PlayerConnected);

			//push messages from palyer1 to player2
			players[1].NetworkAdapter.Received += (s, e) => pushMessageToPlayer(players[0], e);
			players[0].NetworkAdapter.Received += (s, e) => pushMessageToPlayer(players[1], e);

			//initlize the game
			ConnectFourGame game = new ConnectFourGame(players[0], players[1]);
			
		}

		/// <summary>
		/// Decides on the messagetype wether to psuh to the player or not
		/// Pushes a message to the other player
		/// </summary>
		/// <param name="player"></param>
		/// <param name="message"></param>
		private void pushMessageToPlayer(Player player, string serilizedMessage)
		{
			var message = NetworkMessage<object>.DeSerialize(serilizedMessage);
			//send to other wether its a chat message or a movement
			if(message.MessageType == NetworkMessageType.ChatMessage || message.MessageType == NetworkMessageType.Move)
				player.NetworkAdapter.SendMessage(message.Message, message.MessageType);
		}

        private Player parseRequest(string json, Color color, TcpClient client)
        {
			//decode the message
			var msg = NetworkMessage<object>.DeSerialize(json);
			//the first message is always the name
			string name;
			if (msg.MessageType == NetworkMessageType.PlayerName)
				name = msg.Message.ToString();
			else
				name = "Idiot";

			//check if name is taken
			if (players.Any(p => p != null && string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
				name += "_2";

			//instantiate the new player
			var player = new Player(color, name, new NetworkAdapter(client)); //TODO: IP
			//Tell him his color and Name
			player.NetworkAdapter.SendMessage(color, NetworkMessageType.Color);
			player.NetworkAdapter.SendMessage(name, NetworkMessageType.PlayerName);

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
