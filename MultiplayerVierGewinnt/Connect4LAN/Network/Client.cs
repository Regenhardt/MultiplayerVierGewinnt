using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace Connect4LAN.Network
{
	/// <summary>
	/// The client wich connects to the host    
	/// 
	/// Is the User for the User
	/// </summary>
	class Client : NetworkAdapter
	{
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
			this.Received += analyseRecievedMessage;
		}

		private void analyseRecievedMessage(object sender, NetworkMessage e)
		{
			if (e.MessageType == NetworkMessageType.PlayerName)
				this.Name = e.Message.ToString();

			switch (e.MessageType)
			{
				case NetworkMessageType.ServerMessage:									break;
				case NetworkMessageType.ChatMessage:									break;
				case NetworkMessageType.PlayerName:		Name = e.Message.ToString();	break;
				case NetworkMessageType.Color:			Color = (Color)e.Message;		break;
				case NetworkMessageType.Move:											break;
				default:																break;
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
