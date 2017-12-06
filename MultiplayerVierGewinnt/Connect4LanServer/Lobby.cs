using Connect4LAN.Network.Clientside;
using Connect4LAN.Network.Serverside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LanServer
{
	public class Lobby
	{
		private static readonly Color HostColor = Colors.Yellow;
		private static readonly Color ClientColor = Colors.Red;
		public enum GameState
		{
			Open,
			Running,
			Finished
		}
		public GameState State { get; private set; }
		public Player[] Players => players;

		Connect4LAN.Game.ConnectFourGame game;
		Connect4LAN.Network.Serverside.Player[] players;
		

		public Lobby(Player host)
		{
			State = GameState.Open;
			players = new Connect4LAN.Network.Serverside.Player[2];
			players[0] = host;
		}

		public void Start(Player opponent)
		{
			if (State != GameState.Open)
				throw new InvalidOperationException("The game you tried to join isn't open!");
			players[1] = opponent;
			game = new Connect4LAN.Game.ConnectFourGame(players[0], players[1]);

			players[0].NetworkAdapter.SendMessage(null, Connect4LAN.Network.NetworkMessageType.GameStarted);
			players[1].NetworkAdapter.SendMessage(null, Connect4LAN.Network.NetworkMessageType.GameStarted);
		}


	}
}
