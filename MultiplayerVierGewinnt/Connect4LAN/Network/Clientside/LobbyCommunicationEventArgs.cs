using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network.Clientside
{
	class LobbyCommunicationEventArgs : EventArgs
	{
		public LobbyCommunicationEventArgs(LobbyCommunications type, dynamic data)
		{
			this.LobbyCommunicationType = type;
			this.Data = data;
		}

		public LobbyCommunications LobbyCommunicationType { get; private set; }

		public dynamic Data { get; private set; }
	}
}
