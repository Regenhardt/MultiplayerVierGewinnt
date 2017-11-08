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
					placePiece(collumn, row, piece);
					break;
				}
			}

			return row;			
		}

		private void placePiece(int collumn, int row, Piece piece)
		{
			Board[collumn, row] = piece;

			//check other pieces
			//check bottom row
			Piece tmp;

			bool canExistsLeft = collumn != 0, canExistsRight = collumn != (Board.GetLength(0) - 1);
			
			#region [ Check Bottom ]

			//the bottom row
			if (row != 0)
			{
				//bottom middle
				if (IsInitilized(Board[collumn, row - 1]))
				{
					tmp = Board[collumn, row - 1];
					piece.FriendlyAmountBottom = tmp.FriendlyAmountBottom + 1;
				}
				//bottom left
				if (canExistsLeft && IsInitilized(Board[collumn - 1, row - 1]))
				{
					tmp = Board[collumn - 1, row - 1];
					piece.FriendlyAmountBottomLeft = tmp.FriendlyAmountBottomLeft + 1;
					tmp.FriendlyAmountTopRight += 1;
				}
				else
					canExistsLeft = false;

				//bottom right
				if (canExistsRight && IsInitilized(Board[collumn + 1, row - 1]))
				{
					tmp = Board[collumn + 1, row - 1];
					piece.FriendlyAmountBottomLeft = tmp.FriendlyAmountBottomLeft + 1;
					tmp.FriendlyAmountTopRight += 1;
				}
				else
					canExistsRight = false;
			}

			#endregion [ Bottom ]

			#region [ Check Middle ]

			// middle left
			if (canExistsLeft && IsInitilized(Board[collumn - 1, row]))
			{
				tmp = Board[collumn - 1, row];
				piece.FriendlyAmountMiddleLeft = tmp.FriendlyAmountMiddleLeft + 1;
				tmp.FriendlyAmountMiddleRight += 1;
			}
			else
				canExistsLeft = false;
			// middle right
			if (canExistsRight && IsInitilized(Board[collumn + 1, row]))
			{
				tmp = Board[collumn + 1, row];
				piece.FriendlyAmountMiddleRight = tmp.FriendlyAmountMiddleRight + 1;
				tmp.FriendlyAmountMiddleLeft += 1;
			}
			else
				canExistsRight = false;

			#endregion [ check middle ]

			#region [ Check Top ]

			//the top
			if (row != Board.GetLength(1) - 1)
			{
				//top left
				if (canExistsLeft && IsInitilized(Board[collumn - 1, row + 1]))
				{
					tmp = Board[collumn - 1, row + 1];
					piece.FriendlyAmountTopLeft = tmp.FriendlyAmountTopLeft + 1;
					tmp.FriendlyAmountTopRight += 1;
				}
				//top right
				if (canExistsRight && IsInitilized(Board[collumn + 1, row + 1]))
				{
					tmp = Board[collumn + 1, row + 1];
					piece.FriendlyAmountBottomLeft = tmp.FriendlyAmountBottomLeft + 1;
					tmp.FriendlyAmountTopRight += 1;
				}

			}

			#endregion [ Top ]



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
