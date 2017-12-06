using Connect4LAN.Network.Clientside;
using Connect4LAN.Network.Serverside;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LanServer
{
	public class Lobby : INotifyPropertyChanged
	{
		private static readonly Color HostColor = Colors.Yellow;
		private static readonly Color ClientColor = Colors.Red;

		/// <summary>
		/// Fires when the host disconnects before hte game starts.
		/// </summary>
		public event Action<Lobby> HostDisconnected;

		/// <summary>
		/// Fires when the game finishes. Sends the winners name.
		/// </summary>
		public event EventHandler<string> GameFinished;

		/// <summary>
		/// Enums of possible gamestates
		/// </summary>
		public enum GameState
		{
			Open,
			Running,
			Finished
		}		
		
		#region [ State ]

		GameState state;

		public GameState State
		{
			get
			{
				return state;
			}
			set
			{
				if (state != value)
				{
					state = value;
					Notify();
				}
			}
		}

		#endregion 

		public Player[] Players => players;


		public string DisplayState
		{
			get
			{
				return displayState;
			}
			set
			{
				displayState = value;
				Notify();
			}
		}
		private string displayState;

		Connect4LAN.Game.ConnectFourGame game;
		Connect4LAN.Network.Serverside.Player[] players;
		
		/// <summary>
		/// Creates the lobby and sets the given player as host.
		/// </summary>
		/// <param name="host">The first player.</param>
		public Lobby(Player host)
		{
			State = GameState.Open;
			players = new Connect4LAN.Network.Serverside.Player[2];
			players[0] = host;
			host.NetworkAdapter.ConnectionLost += OnHostDisconnected;
			DisplayState = "Open";
		}

		/// <summary>
		/// Start the game against the given player.
		/// </summary>
		/// <param name="opponent">The player to play against.</param>
		public void Start(Player opponent)
		{
			if (State != GameState.Open)
				throw new InvalidOperationException("The game you tried to join isn't open!");
			players[1] = opponent;
			game = new Connect4LAN.Game.ConnectFourGame(players[0], players[1]);
			game.GameOver += OnGameOver;
			DisplayState = "vs";
			players[0].NetworkAdapter.SendMessage(null, Connect4LAN.Network.NetworkMessageType.GameStarted);
			players[1].NetworkAdapter.SendMessage(null, Connect4LAN.Network.NetworkMessageType.GameStarted);
		}

		private void OnGameOver (object sender, string winner)
		{
			DisplayState = string.Equals(winner, players[0].Name) ? "pwnd" : "got pwnd by";
			GameFinished?.Invoke(this, winner);
		}

		private void OnHostDisconnected(object sender, EventArgs args)
		{
			State = GameState.Finished;
			DisplayState = "Aborted";
			HostDisconnected?.Invoke(this);
		}

		// Not sure if this is needed, just in case.
		#region [ INotifyPropertyChanged ]
		private void Notify([CallerMemberName]string property = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
		public event PropertyChangedEventHandler PropertyChanged; 
		#endregion
	}
}
