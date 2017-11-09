using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN.Game
{
	struct Piece
	{
		/// <summary>
		/// THe Color of the piece
		/// </summary>
		public Color Color { get; set; }

		//To save Server Computational power we store it's neighbour
		#region [ SuperPiece ]

		/// <summary>
		/// The pieces to the Bottom of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountBottom { get; set; }

		/// <summary>
		/// The pieces to the Bottom left of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountBottomLeft { get; set; }

		/// <summary>
		/// The pieces to the Bottom right of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountBottomRight { get; set; }


		/// <summary>
		/// The pieces to the Middle of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountMiddle { get; set; }

		/// <summary>
		/// The pieces to the Middle left of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountMiddleLeft { get; set; }

		/// <summary>
		/// The pieces to the Middle right of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountMiddleRight { get; set; }

		/// <summary>
		/// The pieces to the Top left of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountTopLeft { get; set; }

		/// <summary>
		/// The pieces to the Top right of this piece wich have the same color
		/// </summary>
		public int FriendlyAmountTopRight { get; set; }

		#endregion [ SuperPiece ]
	}
}
