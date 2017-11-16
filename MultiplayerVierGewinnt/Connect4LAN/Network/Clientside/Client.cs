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
	class Client : NetworkAdapter
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

		/// <summary>
		/// The Name of the player
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The color of this player, set by the server
		/// </summary>
		public Color Color { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the Player</param>
		public Client(string name = "Player1")
		{
			Name = name;
			base.Received += analyseRecievedMessage;
		}

		#region [ Events ]

		public override event EventHandler<NetworkMessage> Received;

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


		#endregion [ Events ]

		private void analyseRecievedMessage(object sender, NetworkMessage e)
		{
			if (e.MessageType == NetworkMessageType.PlayerName)
				this.Name = e.Message.ToString();

			switch (e.MessageType)
			{
				case NetworkMessageType.ServerMessage:	this.ServerMessageRecieved?.Invoke(this, e.Message.ToString());	break;
				case NetworkMessageType.ChatMessage:	this.ChatMessageRecieved?.Invoke(this, e.Message.ToString()); break;
				case NetworkMessageType.PlayerName:		if (Name != e.Message.ToString()) { Name = e.Message.ToString(); PlayerNamedChanged?.Invoke(this, e.Message.ToString()); };		break;
				case NetworkMessageType.Color:			Color = (Color)ColorConverter.ConvertFromString(e.Message.ToString()); this.ColorChanged?.Invoke(this, Color);		break;
				case NetworkMessageType.Move:			this.MovementRecieved?.Invoke(this, (Move)e.Message);/*Untested */			break;
				case NetworkMessageType.PlayerConnected:this.PlayerJoined?.Invoke(this, (Opponent) e.Message); break;
				default:								this.Received?.Invoke(this, e);									break;
			}
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
