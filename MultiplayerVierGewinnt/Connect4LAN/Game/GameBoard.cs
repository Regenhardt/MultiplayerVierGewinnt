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
			return piece != null;
		}

		/// <summary>
		/// Places piece in Column
		/// </summary>
		/// <param name="column">The column to place the piece in.</param>
		/// <returns>The row in which the piece landed.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If the collum is to big/small</exception>
		/// <exception cref="InvalidOperationException">If the row is full</exception>
		public int PutPiece(int column, Piece piece)
		{
            if (column >= Board.GetLength(0) || column < 0)
				throw new ArgumentOutOfRangeException("Column outisde of board boundaries.");

			//put the piece in the column
			int row;
			for (row = 0; row < Board.GetLength(1); row++)
			{
				//if the field as been initilized then continue to next
				if (!IsInitilized(Board[column, row]))
				{
					//place the field on the board
					Board[column, row] = piece;
			        //and return the row
					return row;
				}
			}

            throw new InvalidOperationException("This column is full.");	
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
