using Connect4LAN.Network;
using Connect4LAN.Network.Clientside;
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
		public event EventHandler<Lobby> LobbyCreated;

		private UdpBroadcaster socket;
		private RequestAcceptor server;
		private List<Lobby> lobbies;
		private List<ClientCommunicator> clients;

		/// <summary>
		/// Instantiates a Dedicated server object
		/// </summary>
		/// <param name="gamePort">The port on wich it shall be listned for incoming requests</param>
		/// <param name="broadcastingPort">The port on wich any discover packets shall be listned for</param>
		public DedicatedServer(int gamePort = 16569, int broadcastingPort = 43133)
		{
			lobbies = new List<Lobby>();
			clients = new List<ClientCommunicator>();

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
			
			while (adapter.ReadLastMessage() == null)
				System.Threading.Thread.Sleep(100);

			////the first message is always the name
			var msg = NetworkMessage<string>.DeSerialize(adapter.ReadLastMessage());
			string name;
			if (msg.MessageType == NetworkMessageType.PlayerName)
				name = msg.Message.ToString();
			else
				name = "Idiot";

			//check if name is taken
			if (clients.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
				name += "_2";

			//player.NetworkAdapter.SendMessage(color, NetworkMessageType.Color);
			adapter.SendMessage(name, NetworkMessageType.PlayerName);

			var commi = new ClientCommunicator(adapter, name);
			commi.LobbyRequestRegisterd += parseLobbyRequest;
			clients.Add(commi);
		}

		private void parseLobbyRequest(object sender, LobbyCommunicationEventArgs e)
		{
			var commi = sender as ClientCommunicator;

			switch (e.LobbyCommunicationType)
			{
				//tell 'im 'bout the lobbies
				case NetworkMessageType.RequestLobbies:
					Dictionary<int, string> dict = new Dictionary<int, string>(lobbies.Count);
					for (int i = 0; i < lobbies.Count; i++)
						if (lobbies[i].State == Lobby.GameState.Open)
							dict.Add(i, lobbies[i].Players.First().Name);
					commi.Adapter.SendMessage(dict, NetworkMessageType.AvailableLobbies);
					break;

				//open new lobby
				case NetworkMessageType.CreateLobby:
					var lobby = new Lobby(new Player(Colors.Yellow, commi.Name, commi.Adapter));
					this.lobbies.Add(lobby);
					LobbyCreated?.Invoke(this, lobby);
					break;

				//wants to join lobby
				case NetworkMessageType.JoinLobby:
					lobbies[e.Data.Value.Message].Start(new Player(Colors.Red, commi.Name, commi.Adapter));
					break;

				//any other message, do nuttin'
				default:
					return;
			}
		}

		/// <summary>
		/// Deals with the communication over UDP
		/// </summary>
		/// <param name="message"></param>
		private void dealWithUdpRequests(string message)
		{
			//replay to the guy
			socket.SendMessage(UdpBroadcaster.GetLocalIPAdress().ToString(), message);
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

		/// <summary>
		/// Class to communicate with client
		/// </summary>
		private class ClientCommunicator
		{
			public NetworkAdapter Adapter { get; }

			public string Name { get; }

			public ClientCommunicator(NetworkAdapter adapter, string playerName)
			{
				this.Adapter = adapter;
				this.Name = playerName;

				adapter.Received += Adapter_Received;
			}

			private void Adapter_Received(object sender, string e)
			{
				var type = NetworkMessage<object>.GetNetworkMessageType(e);

				switch (type)
				{
					//only react on Lobbymessages
					case NetworkMessageType.RequestLobbies:
					case NetworkMessageType.CreateLobby:
						LobbyRequestRegisterd?.Invoke(this, new LobbyCommunicationEventArgs(type, null));
						break;
					case NetworkMessageType.JoinLobby:
						LobbyRequestRegisterd?.Invoke(this, new LobbyCommunicationEventArgs(type, NetworkMessage<int>.DeSerialize(e).Message));
						break;
					//any other message, do nuttin'
					default:
						return;
				}

			}

			public event EventHandler<LobbyCommunicationEventArgs> LobbyRequestRegisterd;
		}




	}
}
