using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN.Network
{
	/// <summary>
	/// The move a player made
	/// </summary>
	struct Move
	{
		/// <summary>
		/// The collumn in wich this piece is set
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// The color of the player who moved, only usefull for spectating
		/// </summary>
		public Color Color { get; set; }
	}
}
