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

			EventHandler<NetworkMessage> handler;

			//wire up player 1
			handler = (s, e) => { if (e.MessageType == NetworkMessageType.Move) executeMove((Move)e.Message, player1); };
			player1.NetworkAdapter.Received += handler;
			player1.NetworkAdapter.ConnectionLost += (s, e) => {
				player2.NetworkAdapter.SendMessage($"{player2.Name} lost connection.{((weHaveAWinner)?"You won by elimination." : "" )}", NetworkMessageType.ServerMessage);
				player1.NetworkAdapter.Received -= handler;
				player1.NetworkAdapter.Disconnect();
			};

			//wire up player 2
			handler = (s, e) => { if (e.MessageType == NetworkMessageType.Move) executeMove((Move)e.Message, player2); };
			player2.NetworkAdapter.Received += handler;
			player1.NetworkAdapter.ConnectionLost += (s, e) => {
				player2.NetworkAdapter.SendMessage($"{player1.Name} lost connection.{((weHaveAWinner)?"You won by elimination." : "" )}", NetworkMessageType.ServerMessage);
				player2.NetworkAdapter.Received -= handler;
				player2.NetworkAdapter.Disconnect();
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

		private void executeMove(Move move, Player player)
		{
			//place his piece
			var piece = placePiece(move, player.Color);
			if (isWinningMove(piece))
			{
				string msg = $"Player {player.Name} has won.";
				weHaveAWinner = true;
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
