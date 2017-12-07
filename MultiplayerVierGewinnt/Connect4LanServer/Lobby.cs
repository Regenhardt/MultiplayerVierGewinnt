using Connect4LAN.Network;
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
		/// <exception cref="InvalidOperationException">When game is full already.</exception>
		public void Start(Player opponent)
		{
			if (State != GameState.Open)
				throw new InvalidOperationException("The game you tried to join isn't open!");
			
			//set oppmmend
			players[1] = opponent;

			//wire up players
			players[1].NetworkAdapter.Received += (s, e) => pushMessageToPlayer(players[0], e);
			players[0].NetworkAdapter.Received += (s, e) => pushMessageToPlayer(players[1], e);

			//start and tell them it started
			game = new Connect4LAN.Game.ConnectFourGame(players[0], players[1]);
			players[0].NetworkAdapter.SendMessage(null, NetworkMessageType.GameStarted);
			players[1].NetworkAdapter.SendMessage(null, NetworkMessageType.GameStarted);

			//listen to finish event
			game.GameOver += OnGameOver;
			DisplayState = "vs"; ;
			this.State = GameState.Running;
		}

		/// <summary>
		/// Decides on the messagetype wether to psuh to the player or not
		/// Pushes a message to the other player
		/// </summary>
		/// <param name="player"></param>
		/// <param name="message"></param>
		private void pushMessageToPlayer(Player player, string serilizedMessage)
		{
			var message = NetworkMessage<object>.DeSerialize(serilizedMessage);
			//send to other wether its a chat message or a movement
			if (message.MessageType == NetworkMessageType.ChatMessage)
				player.NetworkAdapter.SendMessage(message.Message, NetworkMessageType.ChatMessage);
			else if (message.MessageType == NetworkMessageType.Move)
			{
				player.NetworkAdapter.SendMessage(NetworkMessage<Move>.DeSerialize(serilizedMessage).Message, NetworkMessageType.Move);
			}
		}

		private void OnGameOver (object sender, string winner)
		{
			DisplayState = string.Equals(winner, players[0].Name) ? "pwnd" : "got pwnd by";
			GameFinished?.Invoke(this, winner);
			State = GameState.Finished;
		}

		private void OnHostDisconnected(object sender, EventArgs args)
		{
			if(State != GameState.Finished)
			{
				State = GameState.Finished;
				DisplayState = "Aborted";
				HostDisconnected?.Invoke(this);
			}
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
