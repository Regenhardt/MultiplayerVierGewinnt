using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network.Clientside
{
	interface ILobbyCommunicator
	{
		event EventHandler<LobbyCommunicationEventArgs> LobbyEventRegisterd;
	}
}
