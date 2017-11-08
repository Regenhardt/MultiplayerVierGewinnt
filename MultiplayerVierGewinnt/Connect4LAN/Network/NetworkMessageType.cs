﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
	/// <summary>
	/// The type of mesage sent thrtough the network
	/// </summary>
	enum NetworkMessageType
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
		Move
	}
}
