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
				var e = NetworkMessage<object>.DeSerilize(msg);
				if (e.MessageType == NetworkMessageType.Move)
					executeMove(NetworkMessage<Move>.DeSerilize(msg).Message, player1);
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
				var e = NetworkMessage<object>.DeSerilize(msg);
				if (e.MessageType == NetworkMessageType.Move)
					executeMove(NetworkMessage<Move>.DeSerilize(msg).Message, player2);
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
				if (isWinningMove(piece))
				{
					string msg = $"Player {player.Name} has won.";
					writeMessageToPlayers(msg);
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
			Piece piece = new Piece{ Color = color };
			this.Gameboard.PutPiece(move.Column, piece);
			return piece;			
		}

		private void calculateNeighbours(int collumn, int row, Piece piece)
		{
			//check other pieces
			//check bottom row
			Piece tmp;
			var Board = Gameboard.Board;

			bool canExistsLeft = collumn != 0, canExistsRight = collumn != (Board.GetLength(0) - 1);

			#region [ Check Bottom ]

			//the bottom row
			if (row != 0)
			{
				//bottom middle
				if (Gameboard.IsInitilized(Board[collumn, row - 1]))
				{
					tmp = Board[collumn, row - 1];
					piece.FriendlyAmountBottom = tmp.FriendlyAmountBottom + 1;
				}
				//bottom left
				if (canExistsLeft && Gameboard.IsInitilized(Board[collumn - 1, row - 1]))
				{
					tmp = Board[collumn - 1, row - 1];
					piece.FriendlyAmountBottomLeft = tmp.FriendlyAmountBottomLeft + 1;
					tmp.FriendlyAmountTopRight += 1;
				}
				else
					canExistsLeft = false;

				//bottom right
				if (canExistsRight && Gameboard.IsInitilized(Board[collumn + 1, row - 1]))
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
			if (canExistsLeft && Gameboard.IsInitilized(Board[collumn - 1, row]))
			{
				tmp = Board[collumn - 1, row];
				piece.FriendlyAmountMiddleLeft = tmp.FriendlyAmountMiddleLeft + 1;
				tmp.FriendlyAmountMiddleRight += 1;
			}
			else
				canExistsLeft = false;
			// middle right
			if (canExistsRight && Gameboard.IsInitilized(Board[collumn + 1, row]))
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
				if (canExistsLeft && Gameboard.IsInitilized(Board[collumn - 1, row + 1]))
				{
					tmp = Board[collumn - 1, row + 1];
					piece.FriendlyAmountTopLeft = tmp.FriendlyAmountTopLeft + 1;
					tmp.FriendlyAmountTopRight += 1;
				}
				//top right
				if (canExistsRight && Gameboard.IsInitilized(Board[collumn + 1, row + 1]))
				{
					tmp = Board[collumn + 1, row + 1];
					piece.FriendlyAmountBottomLeft = tmp.FriendlyAmountBottomLeft + 1;
					tmp.FriendlyAmountTopRight += 1;
				}

			}

			#endregion [ Top ]



		}


		/// <summary>
		/// Checks if the color of this piece has connected 4.
		/// </summary>
		/// <param name="piece"></param>
		private bool isWinningMove(Piece piece)
		{
			int[] allProps = new int[]
			{
				piece.FriendlyAmountBottom,
				piece.FriendlyAmountBottomLeft,
				piece.FriendlyAmountBottomRight,
				piece.FriendlyAmountMiddle,
				piece.FriendlyAmountMiddleLeft,
				piece.FriendlyAmountMiddleRight,
				piece.FriendlyAmountTopLeft,
				piece.FriendlyAmountTopRight
			};

			return allProps.Any(i => i >= 4);
		}
	}    
}
