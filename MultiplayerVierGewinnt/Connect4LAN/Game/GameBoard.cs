using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Game
{
	class Gameboard
	{
		/// <summary>
		/// The actual game baoard
		/// </summary>
		public Piece[,] Board { get; private set; }

		public Gameboard()
		{
			this.ClearField();
		}

		/// <summary>
		/// Places piece in Column
		/// </summary>
		/// <param name="collumn"></param>
		/// <returns></returns>
		public int PutPiece(int collumn, Piece piece)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Resets the game/clears the field
		/// </summary>
		public void ClearField()
		{
			this.Board = new Piece[6, 7];
		}
	}
}
