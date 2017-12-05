using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LanServer.Network
{

	class DedicatedServer
	{
		private UdpBroadcaster socket;

		/// <summary>
		/// Instantiates a Dedicated server object
		/// </summary>
		/// <param name="gamePort">The port on wich it shall be listned for incoming requests</param>
		/// <param name="broadcastingPort"></param>
		public DedicatedServer(int gamePort = 16569, int broadcastingPort = 43133)
		{
			//start listning for incoming broadcasts
			socket = new UdpBroadcaster(broadcastingPort);

		}
	}
}
