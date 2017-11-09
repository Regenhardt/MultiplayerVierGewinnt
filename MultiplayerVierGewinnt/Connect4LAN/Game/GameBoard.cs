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
		/// Checkes wether de piece is null -> default
		/// </summary>
		/// <param name="piece"></param>
		/// <returns></returns>
		public bool IsInitilized(Piece piece)
		{
			return !piece.Equals(default(Piece));
		}

		/// <summary>
		/// Places piece in Column
		/// </summary>
		/// <param name="collumn"></param>
		/// <returns></returns>
		public int PutPiece(int collumn, Piece piece)
		{
			//put the piece in the coloum
			int row;
			for (row = 0; row < Board.GetLength(1); row++)
			{
				//if the field as been initilized then continue to next
				if (!IsInitilized(Board[collumn, row]))
				{
					//place the field on the board
					Board[collumn, row] = piece;
					break;
				}
			}

			//and return the row
			return row;			
		}

		/// <summary>
		/// Resets the game/clears the field
		/// </summary>
		public void ClearField()
		{
			this.Board = new Piece[7, 6];
		}
	}
}
