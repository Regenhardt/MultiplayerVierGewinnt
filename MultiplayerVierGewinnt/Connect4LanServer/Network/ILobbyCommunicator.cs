﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LanServer.Network
{
	interface ILobbyCommunicator
	{
		event EventHandler<LobbyCommunicationEventArgs> LobbyEventRegisterd;
	}
}
