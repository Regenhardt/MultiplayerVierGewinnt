using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network.Clientside
{
	class LobbyCommunicationEventArgs : EventArgs
	{
		public LobbyCommunicationEventArgs(NetworkMessageType type, dynamic data)
		{
			this.LobbyCommunicationType = type;
			this.Data = data;
		}

		public NetworkMessageType LobbyCommunicationType { get; private set; }

		public NetworkMessage<int>? Data { get; private set; }
	}
}
