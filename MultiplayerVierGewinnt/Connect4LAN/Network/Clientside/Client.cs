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

namespace Connect4LAN.Network.Clientside
{
	/// <summary>
	/// The client wich connects to the host    
	/// 
	/// Is the User for the User
	/// </summary>
	public class Client : NetworkAdapter
	{
		/// <summary>
		/// The Current users IPadress
		/// </summary>
		/// <exception cref="IOException">When the host is not connected to a network</exception>
		public new IPAddress IP
		{
			get
			{
				if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
					throw new IOException("Not connected to a network.");
					
				return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);				
			}
		}

		public string ServerIp;

		private Dictionary<int, string> LobbyGames;

		/// <summary>
		/// The Name of the player
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The color of this player, set by the server
		/// </summary>
		public Color Color { get; private set; }
		public event EventHandler<Dictionary<string, string>> AvailableLobbies;

		#region [ Constructor ]

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the Player</param>
		public Client(string name = "Player1")
		{
			Name = name;
			base.Received += analyseRecievedMessage;
		} 

		#endregion

		#region [ Events ]

		public override event EventHandler<string> Received;

		/// <summary>
		/// A Chat message was recieved through the network
		/// </summary>
		public event EventHandler<string> ChatMessageRecieved;
		/// <summary>
		/// The server assigned us a new playername
		/// </summary>
		public event EventHandler<string> PlayerNamedChanged;
		/// <summary>
		/// A Server message was recieved through the network
		/// </summary>
		public event EventHandler<string> ServerMessageRecieved;
		/// <summary>
		/// The Oponnent connected and the game can start
		/// </summary>
		public event EventHandler<Opponent> PlayerJoined;
		/// <summary>
		/// The server assigned us a color
		/// </summary>
		public event EventHandler<Color> ColorChanged;
		/// <summary>
		/// The oponnent placed a piece on the GameBoard
		/// </summary>
		public event EventHandler<Move> MovementRecieved;
		/// <summary>
		/// Indicates when the game is over, is true if user won
		/// </summary>
		public event EventHandler<bool> GameOver;
		/// <summary>
		/// Fires when no dedicated server could be found.
		/// </summary>
		public event EventHandler<string> ServerNotFound;
		/// <summary>
		/// Fires when the client successfully connected to a server.
		/// </summary>
		public event EventHandler ConnectedToServer;


		#endregion [ Events ]

		private void analyseRecievedMessage(object sender, string serilizedMessage)
		{
			var msg = NetworkMessage<object>.DeSerialize(serilizedMessage);
			switch (msg.MessageType)
			{
				case NetworkMessageType.ServerMessage:		this.ServerMessageRecieved?.Invoke(this, msg.Message.ToString());	break;
				case NetworkMessageType.ChatMessage:		this.ChatMessageRecieved?.Invoke(this, msg.Message.ToString()); break;
				case NetworkMessageType.PlayerName:			if (Name != msg.Message.ToString()) { Name = msg.Message.ToString(); PlayerNamedChanged?.Invoke(this, msg.Message.ToString()); };		break;
				case NetworkMessageType.Color:				Color = NetworkMessage<Color>.DeSerialize(serilizedMessage).Message; this.ColorChanged?.Invoke(this, Color);		break;
				case NetworkMessageType.Move:				this.MovementRecieved?.Invoke(this, NetworkMessage<Move>.DeSerialize(serilizedMessage).Message);			break;
				case NetworkMessageType.PlayerConnected:	this.PlayerJoined?.Invoke(this, NetworkMessage<Opponent>.DeSerialize(serilizedMessage).Message); break;
				case NetworkMessageType.GameOver:			this.GameOver?.Invoke(this, NetworkMessage<bool>.DeSerialize(serilizedMessage).Message); break;
				case NetworkMessageType.AvailableLobbies:	this.AvailableLobbies?.Invoke(this, NetworkMessage<Dictionary<string, string>>.DeSerialize(serilizedMessage).Message); break;
				default: this.Received?.Invoke(this, serilizedMessage);									break;
			}
		}

		public Dictionary<string, string> GetLobbies()
		{
			SendMessage(null, NetworkMessageType.RequestLobbies);

			bool lobbiesReceived = false;
			Dictionary<string, string> lobbies = null;
			this.AvailableLobbies += (s, ls) => { lobbies = ls; lobbiesReceived = true; };

			while (!lobbiesReceived) System.Threading.Thread.Sleep(10);

			return lobbies;
		}

		/// <summary>
		/// Sends a chat message to the server
		/// </summary>
		/// <param name="msg"></param>
		public void SendMessage(string msg)
		{
			base.SendMessage(msg, NetworkMessageType.ChatMessage);
		}

		/// <summary>
		/// Starts the connection process.
		/// Make sure the ServerNotFound and ConnectedToServer events have handlers before you call this.
		/// </summary>
		public void ConnectToDedicatedServer()
		{
			new System.Threading.Thread(FindAndConnectToServer).Start();
		}

		private void FindAndConnectToServer()
		{
			string serverIP;

			try
			{
				serverIP = UdpBroadcaster.FindGameServer();
				ServerIp = serverIP;
				if (!Connect(serverIP))
					ServerNotFound?.Invoke(this, "Server was found but didn't respond.");
				else
					ConnectedToServer?.Invoke(this, EventArgs.Empty);
			}
			catch (ServerNotFoundException ex)
			{
				ServerNotFound?.Invoke(this, ex.Message);
			}
			
		}

		/// <summary>
		/// Connects to designated IP Adresses.
		/// Catchhes IO Exceptions and returns false in that case. Else it throws the error
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		override public bool Connect(string ipAddress, int port = 16569)
		{
			//connect the player
			if (base.Connect(ipAddress, port))
				this.SendMessage(Name, NetworkMessageType.PlayerName);
			//and send if it worked
			else
				return false;
			return true;
		}
	}
}
