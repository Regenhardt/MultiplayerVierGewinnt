using Connect4LAN.Network;
using Connect4LAN.Network.Serverside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN.Game
{
	/// <summary>
	/// Represents a Connect4 Game instance.
	/// </summary>
	class ConnectFourGame
	{
		public Player player1 { get; private set; }
		public Player player2 { get; private set; }

		public Gameboard Gameboard { get; private set; }

		//Flag to dermin wether we ahve a winner
		private bool weHaveAWinner = false;

		/// <summary>
		/// Instaniates a Game of Connect 
		/// </summary>
		/// <param name="player1"></param>
		/// <param name="player2"></param>
		public ConnectFourGame(Player player1, Player player2)
		{
			//set variables
			this.player1 = player1;
			this.player2 = player2;
			this.Gameboard = new Gameboard();

			EventHandler<string> handler;

			//wire up player 1
			handler = (s, msg) => 
			{
				var e = NetworkMessage<object>.DeSerialize(msg);
				if (e.MessageType == NetworkMessageType.Move)
					executeMove(NetworkMessage<Move>.DeSerialize(msg).Message, player1);
			};
			player1.NetworkAdapter.Received += handler;
			player1.NetworkAdapter.ConnectionLost += (s, e) =>
			{
				player2.NetworkAdapter.SendMessage($"{player2.Name} lost connection.{((!weHaveAWinner)?"You won by elimination." : "" )}", NetworkMessageType.ServerMessage);
				player1.NetworkAdapter.Received -= handler;
				player2.NetworkAdapter.Disconnect();
			};

			//wire up player 2
			handler = (s, msg) =>
			{
				var e = NetworkMessage<object>.DeSerialize(msg);
				if (e.MessageType == NetworkMessageType.Move)
					executeMove(NetworkMessage<Move>.DeSerialize(msg).Message, player2);
			};
			player2.NetworkAdapter.Received += handler;
			player1.NetworkAdapter.ConnectionLost += (s, e) => 
			{
				player1.NetworkAdapter.SendMessage($"{player1.Name} lost connection.{((!weHaveAWinner)?"You won by elimination." : "" )}", NetworkMessageType.ServerMessage);
				player2.NetworkAdapter.Received -= handler;
				player1.NetworkAdapter.Disconnect();
			};
		}

		/// <summary>
		/// Writes the message to both players
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="type"></param>
		private void writeMessageToPlayers(string msg, NetworkMessageType type = NetworkMessageType.ServerMessage)
		{
			this.player1.NetworkAdapter.SendMessage(msg, type);
			this.player2.NetworkAdapter.SendMessage(msg, type);
		}

		/// <summary>
		/// Executes the movement on the gameboard
		/// </summary>
		/// <param name="move"></param>
		/// <param name="player"></param>
		private void executeMove(Move move, Player player)
		{
			//place piece aslont as no1 won
			if (!weHaveAWinner)
			{
				var piece = placePiece(move, player.Color);
				if (piece.IsWinningPiece)
				{
					//send out who won
					string msg = $"Player {player.Name} has won.";
					writeMessageToPlayers(msg);

					//fire the event
					player.NetworkAdapter.SendMessage(true, NetworkMessageType.GameOver);
					((player1 == player)? player2: player1).NetworkAdapter.SendMessage(false, NetworkMessageType.GameOver);

					//set that we indeed have a winner
					weHaveAWinner = true;
				}
			}
		}

		/// <summary>
		/// Places a piece wich is derived from move struct
		/// </summary>
		/// <param name="move"></param>
		/// <param name="color"></param>
		private Piece placePiece(Move move, Color color)
		{
			//iniate piec eon board
			Piece piece = new Piece{ Color = color };
			//place it in its collum/row
			var row = Gameboard.PutPiece(move.Column, piece);
			//update the neighboring pieces
			calculateNeighbours(move.Column, row, piece);
			//return the piece
			return piece;			
		}

		//calculates the neighbors of a piece
		private void calculateNeighbours(int collumn, int row, Piece piece)
		{
			piece.IsWinningPiece =
				(calculateLeft(collumn - 1, row, piece) + calculateRight(collumn + 1, row, piece) >= 3)					||
				(calculateTop(collumn, row + 1, piece) + calculateBottom(collumn, row - 1, piece) >= 3) ||
				(calculateTopLeft(collumn - 1, row + 1, piece) + calculateBottomRight(collumn + 1, row - 1, piece) >= 3) ||
				(calculateTopRight(collumn + 1, row + 1, piece) + calculateBottomLeft(collumn - 1, row - 1, piece) >= 3);			
		}

		/// <summary>
		/// Old way of calculaiton kept for reasons unknown
		/// </summary>
		/// <param name="collumn"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		private void calculateNeighbour(int collumn, int row, Piece piece)
		{
			//check other pieces
			//check bottom row
			Piece p1, p2;
			var Board = Gameboard.Board;

			bool canExistsLeft = collumn != 0, canExistsRight = collumn != (Board.GetLength(0) - 1);
			bool canExistsBelow = row != 0, canExistsAbove = row != (Board.GetLength(1) - 1);
			bool topLeftChecked = false, topRightChecked = false;
			
			//checks the piece on the oard if its freindly
			Func<int, int, Piece, bool> pieceIsFriendly = new Func<int, int, Piece, bool>((co, ro, pi) => 
			{
				try
				{
					//check if peice is initilized
					if (!Gameboard.IsInitilized(Board[co, ro]))
						return false;

					//return if both colors are equal
					return Board[co, ro].Color == pi.Color;
				}
				catch (IndexOutOfRangeException)
				{
					return false;
				}
			});

			#region [ Check Bottom ]

			//the bottom row
			if (canExistsBelow)
			{
				//bottom middle
				if (Gameboard.IsInitilized(Board[collumn, row - 1]))
				{
					p1 = Board[collumn, row - 1];
					if(p1.Color == piece.Color)
						piece.FriendlyAmountBottom = p1.FriendlyAmountBottom + 1;
				}
				//bottom left
				if (canExistsLeft && Gameboard.IsInitilized(Board[collumn - 1, row - 1]))
				{
					p1 = Board[collumn - 1, row - 1];
					//TODO moaa calcu
					if (p1.Color == piece.Color)
					{
						//check if a topright piece exists
						if(canExistsRight && canExistsAbove && pieceIsFriendly(collumn + 1, row + 1, piece))
						{
							//get 2nd piece and adjust movement
							p2 = Board[collumn + 1, row + 1];
							p1.FriendlyAmountTopRight = p2.FriendlyAmountTopRight + 1;
							p2.FriendlyAmountBottomLeft = p1.FriendlyAmountBottomLeft + 1;

							//Don't have to set bottom/top right/left from current piece, because it will never ever be read from again
							piece.IsWinningPiece = p1.FriendlyAmountTopRight >= 4 || p2.FriendlyAmountBottomLeft >= 4;

							topRightChecked = true;
						}
						//no friendly top right exists
						else
						{
							//set pieces
							p1.FriendlyAmountTopRight += 1;
							piece.FriendlyAmountBottomLeft = p1.FriendlyAmountBottomLeft + 1;

							//set the winning piece
							piece.IsWinningPiece = p1.FriendlyAmountTopRight >= 4 || piece.FriendlyAmountBottomLeft >= 4;
						}

					}
				}
				else
					canExistsLeft = false;

				//bottom right
				if (canExistsRight && Gameboard.IsInitilized(Board[collumn + 1, row - 1]))
				{
					p1 = Board[collumn + 1, row - 1];
					//TODO moaa calcu
					if (p1.Color == piece.Color)
					{
						//check if a topright piece exists
						if (canExistsLeft && canExistsAbove && pieceIsFriendly(collumn - 1, row + 1, piece))
						{
							//get 2nd piece and adjust movement
							p2 = Board[collumn - 1, row + 1];
							p1.FriendlyAmountTopLeft = p2.FriendlyAmountTopLeft + 1;
							p2.FriendlyAmountBottomRight = p1.FriendlyAmountBottomRight + 1;

							//Don't have to set bottom/top right/left from current piece, because it will never ever be read from again
							piece.IsWinningPiece = p1.FriendlyAmountTopLeft >= 4 || p2.FriendlyAmountBottomRight >= 4;

							topLeftChecked = true;
						}
						//no friendly top right exists
						else
						{
							//set pieces
							p1.FriendlyAmountTopLeft += 1;
							piece.FriendlyAmountBottomRight = p1.FriendlyAmountBottomLeft + 1;

							//set the winning piece
							piece.IsWinningPiece = p1.FriendlyAmountTopLeft >= 4 || piece.FriendlyAmountBottomRight >= 4;
						}
					}
				}
				else
					canExistsRight = false;
			}

			#endregion [ Bottom ]

			#region [ Check Middle ]

			// middle left
			if (canExistsLeft && Gameboard.IsInitilized(Board[collumn - 1, row]))
			{
				p1 = Board[collumn - 1, row];
				//TODO moaa calcu
				if (p1.Color == piece.Color)
				{
					piece.FriendlyAmountMiddleLeft = p1.FriendlyAmountMiddleLeft + 1;
					p1.FriendlyAmountMiddleRight += 1;

					//set if it was the winning piece
					piece.IsWinningPiece = piece.FriendlyAmountMiddleLeft >= 4 || p1.FriendlyAmountMiddleRight >= 4;
				}
			}
			else
				canExistsLeft = false;

			// middle right
			if (canExistsRight && Gameboard.IsInitilized(Board[collumn + 1, row]))
			{
				p1 = Board[collumn + 1, row];
					//TODO moaa calcu
				if (p1.Color == piece.Color)
				{
					piece.FriendlyAmountMiddleRight = p1.FriendlyAmountMiddleRight + 1;
					p1.FriendlyAmountMiddleLeft += 1;

					//set if it was the winning piece
					piece.IsWinningPiece = piece.FriendlyAmountMiddleRight >= 4 || p1.FriendlyAmountMiddleLeft >= 4;
				}
			}
			else
				canExistsRight = false;

			#endregion [ check middle ]

			#region [ Check Top ]

			//the top
			if (canExistsAbove)
			{
				//top left
				if (canExistsLeft && !topLeftChecked && Gameboard.IsInitilized(Board[collumn - 1, row + 1]))
				{
					p1 = Board[collumn - 1, row + 1];

					//TODO moaa calcu
					if (p1.Color == piece.Color)
					{
						piece.FriendlyAmountTopLeft = p1.FriendlyAmountTopLeft + 1;
						p1.FriendlyAmountBottomRight += 1;

						//set if it was the winning piece
						piece.IsWinningPiece = piece.FriendlyAmountTopLeft >= 4 || p1.FriendlyAmountBottomRight >= 4;
					}
				}

				//top right
				if (canExistsRight && !topRightChecked && Gameboard.IsInitilized(Board[collumn + 1, row + 1]))
				{
					p1 = Board[collumn + 1, row + 1];

					//TODO moaa calcu
					if (p1.Color == piece.Color)
					{
						piece.FriendlyAmountTopRight = p1.FriendlyAmountTopRight + 1;
						p1.FriendlyAmountBottomLeft += 1;

						//set if it was the winning piece
						piece.IsWinningPiece = piece.FriendlyAmountTopRight >= 4 || p1.FriendlyAmountBottomLeft >= 4;
					}
				}

			}

			#endregion [ Top ]

		}


		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateLeft(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateLeft(collum - 1, row, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;				
			}
		}

		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateTopLeft(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateTopLeft(collum - 1, row + 1, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateBottomLeft(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateBottomLeft(collum - 1, row - 1, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateBottom(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateBottom(collum, row - 1, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateTop(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateBottom(collum, row + 1, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateBottomRight(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateBottomRight(collum + 1, row - 1, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateRight(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateRight(collum + 1, row, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Calculates the left row
		/// </summary>
		/// <param name="collum"></param>
		/// <param name="row"></param>
		/// <param name="piece"></param>
		/// <returns></returns>
		private int calculateTopRight(int collum, int row, Piece piece)
		{
			try
			{
				Piece tmp = Gameboard.Board[collum, row];
				if (Gameboard.IsInitilized(tmp))
				{
					if (tmp.Color == piece.Color)
						return 1 + calculateTopRight(collum + 1, row + 1, piece);
					else
						return 0;
				}
				else
					return 0;

			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}


	}
}
