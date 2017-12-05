using Connect4LAN.Network;
using Connect4LAN.Network.Serverside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LanServer.Network
{

	class DedicatedServer
	{
		private UdpBroadcaster socket;
		private RequestAcceptor server;
		private List<Lobby> lobbies;

		/// <summary>
		/// Instantiates a Dedicated server object
		/// </summary>
		/// <param name="gamePort">The port on wich it shall be listned for incoming requests</param>
		/// <param name="broadcastingPort">The port on wich any discover packets shall be listned for</param>
		public DedicatedServer(int gamePort = 16569, int broadcastingPort = 43133)
		{
			//build the server
			this.server = new RequestAcceptor(gamePort);

			//start listning for incoming broadcasts
			this.socket = new UdpBroadcaster(broadcastingPort);

			//deal with any clients that connect
			server.ClientConnected += (s, e) => dealWithTcpRequests(e);
			socket.MessageRecieved += (s, e) => dealWithUdpRequests(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="client"></param>
		private void dealWithTcpRequests(TcpClient client)
		{
			//temporary set color and name cuz i am lazy
			NetworkAdapter adapter = new NetworkAdapter(client);
			


			//decode the message
			//var msg = NetworkMessage<object>.DeSerialize(json);
			////the first message is always the name
			//string name;
			//if (msg.MessageType == NetworkMessageType.PlayerName)
			//	name = msg.Message.ToString();
			//else
			//	name = "Idiot";

			////check if name is taken
			//if (players.Any(p => p != null && string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
			//	name += "_2";

			//player.NetworkAdapter.SendMessage(color, NetworkMessageType.Color);
			//player.NetworkAdapter.SendMessage(name, NetworkMessageType.PlayerName);

			////return him
			//return player;
		}

		/// <summary>
		/// Deals with the communication over UDP
		/// </summary>
		/// <param name="message"></param>
		private void dealWithUdpRequests(string message)
		{
			//replay to the guy
			socket.SendMessage(server.IP.MapToIPv4().ToString(), message);
		}

		/// <summary>
		/// Handles any request for Tcp connection requests
		/// </summary>
		private class RequestAcceptor : IDisposable
		{
			private bool run = true;

			public TcpListener Socket { get; private set; }
			public IPAddress IP { get; private set; }

			public RequestAcceptor(int port)
			{
				//build socket
				IP = IPAddress.Any;
				this.Socket = new TcpListener(IP, port);
				Socket.Start();

				//accept peeps
				Task.Run(() =>
				{
					try
					{
						while(run)
							ClientConnected?.Invoke(this, Socket.AcceptTcpClient());
					}
					catch (Exception)
					{
						Stopped?.Invoke();
						if (Socket != null)
							Socket.Stop();
					}
				});
			}

			public event EventHandler<TcpClient> ClientConnected;

			public delegate void StoppedEventHandler();
			public event StoppedEventHandler Stopped; 

			public void Dispose()
			{
				if (Socket != null)
					Socket.Stop();
			}


		}

	}
}
