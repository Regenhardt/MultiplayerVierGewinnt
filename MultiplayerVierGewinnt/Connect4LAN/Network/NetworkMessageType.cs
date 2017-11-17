using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
	/// <summary>
	/// The type of mesage sent thrtough the network
	/// </summary>
	public enum NetworkMessageType
	{
		/// <summary>
		/// A Message from the server to display to the client
		/// </summary>
		ServerMessage,

		/// <summary>
		/// A Chat message send by a Player
		/// </summary>
		ChatMessage,

		/// <summary>
		/// The Playername
		/// </summary>
		PlayerName,	

		/// <summary>
		/// A Color
		/// </summary>
		Color,

		/// <summary>
		/// A Move
		/// </summary>
		Move,
		
		/// <summary>
		/// An opponent has connected
		/// </summary>
		PlayerConnected,

		
	}

	public static class NetworkMessageTypeExtensions
	{
		/// <summary>
		/// Converts the Enum to the corresponing Type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="FormatException">On any converstion that is new</exception>
		public static Type ToType(this NetworkMessageType type)
		{
			switch (type)
			{
				case NetworkMessageType.ServerMessage:
					return typeof(string);
				case NetworkMessageType.ChatMessage:
					return typeof(string);
				case NetworkMessageType.PlayerName:
					return typeof(string);
				case NetworkMessageType.Color:
					return typeof(System.Windows.Media.Color);
				case NetworkMessageType.Move:
					return typeof(Move);
				case NetworkMessageType.PlayerConnected:
					return typeof(Opponent);
				default:
					throw new FormatException("Conversion for this type hasn't been implemented yet.");
			}
		}
	}
}
