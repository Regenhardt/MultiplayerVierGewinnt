using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN.Network.Serverside
{
	/// <summary>
	/// The connected Player Serverside
	/// 
	/// Is the user for the Server
	/// </summary>
	public class Player
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color">The Color of the Player</param>
		/// <param name="name">The name of the Player</param>
		/// <param name="ip">The IP with wich he is connected</param>
		/// <param name="socket">The Socket on wich e is connected</param>
		/// <exception cref="ArgumentNullException"/>
		public Player(Color color, string name, NetworkAdapter adapter )
		{
			if (adapter == null)
				throw new ArgumentNullException(nameof(adapter));

			this.Color = color;
			this.Name = name;
			this.NetworkAdapter = adapter;			
		}

		#region  [ Properties ]

		public Color Color { get; private set; }
		public string Name { get; private set; }

		public NetworkAdapter NetworkAdapter { get; private set; }

		#endregion

		#region [ Methods ]

		public void Won(string message = "You win :)")
		{
			this.NetworkAdapter.SendMessage(message, NetworkMessageType.ServerMessage);
			this.NetworkAdapter.SendMessage(true, NetworkMessageType.GameOver);
			this.NetworkAdapter.Disconnect();
		}

		#endregion

	}
}
