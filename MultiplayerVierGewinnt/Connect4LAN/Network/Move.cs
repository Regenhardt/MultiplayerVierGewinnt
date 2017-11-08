using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
