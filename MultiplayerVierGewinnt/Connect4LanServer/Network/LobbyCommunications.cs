using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LanServer.Network
{
	/// <summary>
	/// Holds the different types of communication between client and Server before the game starts.
	/// </summary>
	enum LobbyCommunications
	{
		/// <summary>
		/// The Game starts now.
		/// No payload.
		/// </summary>
		GameStarted,
		/// <summary>
		/// Broadcast to find running dedicated server.
		/// No payload.
		/// </summary>
		Discover,
		/// <summary>
		/// Queries the server for available lobbies.
		/// Payload is <see cref="Dictionary{int, string}"/>
		/// </summary>
		AvailableLobbies,
		/// <summary>
		/// Client wants to join a particular lobby.
		/// Payload is int of the player to join.
		/// </summary>
		JoinLobby,
		/// <summary>
		/// Client wants to create a new lobby.
		/// No payload.
		/// </summary>
		CreateLobby
	}
}
