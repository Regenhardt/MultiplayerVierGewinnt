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
    /// Represents a Connect4 Game instance
    /// </summary>
    class ConnectFourGame
    {
        public Player player1 { get; private set; }
        public Player player2 { get; private set; }

		public Gameboard Gameboard { get; private set; }

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

			//listen to moves by the player
			player1.NetworkAdapter.Received += (s, e) => { if (e.MessageType == NetworkMessageType.Move) placePiece((Move)e.Message, player1.Color); };
			player2.NetworkAdapter.Received += (s, e) => { if (e.MessageType == NetworkMessageType.Move) placePiece((Move)e.Message, player2.Color); };
		}

		/// <summary>
		/// Places a piece wich is derived from move struct
		/// </summary>
		/// <param name="move"></param>
		/// <param name="color"></param>
		private void placePiece(Move move, Color color)
		{
			Piece piece = new Piece{ Color = color };
			this.Gameboard.PutPiece(move.Column, piece);

			checkIfPlayerWon(piece);
		}

		/// <summary>
		/// Checks if the color of this piece has connected 4.
		/// </summary>
		/// <param name="piece"></param>
		private void checkIfPlayerWon(Piece piece)
		{
			throw new NotImplementedException();
		}
	}    
}
