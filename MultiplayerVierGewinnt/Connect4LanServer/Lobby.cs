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

		Connect4LAN.Game.ConnectFourGame game;
		Connect4LAN.Network.Serverside.Player[] players;

		public Lobby(Connect4LAN.Network.Clientside.Client host)
		{
			State = GameState.Open;
			players = new Connect4LAN.Network.Serverside.Player[2];
			players[0] = new Player(HostColor, host.Name, host);
		}

		public void Start(Client opponent)
		{
			if (State != GameState.Open) throw new InvalidOperationException("The game you tried to join isn't open!");
			players[1] = new Player(ClientColor, opponent.Name, opponent);
			game = new Connect4LAN.Game.ConnectFourGame(players[0], players[1]);
		}
	}
}
