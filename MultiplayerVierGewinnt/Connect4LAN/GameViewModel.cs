using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Connect4LAN.Network;
using System.Collections.Concurrent;
using System.Windows.Threading;
using Connect4LAN.Network.Clientside;

namespace Connect4LAN
{
	internal class GameViewModel : INotifyPropertyChanged, IDisposable
	{

		#region [ Commands ]

		/// <summary>
		/// Host a game
		/// </summary>
		public ICommand HostGameCommand
		{
			get
			{
				if (hostGameCommand == null) hostGameCommand = new RelayCommand(param => HostGame());
				return hostGameCommand;
			}
		}
		private RelayCommand hostGameCommand;

		/// <summary>
		/// Join a game
		/// </summary>
		public ICommand JoinGameCommand
		{
			get
			{
				if (joinGameCommand == null) joinGameCommand = new RelayCommand(param => JoinGame());
				return joinGameCommand;
			}
		}
		private RelayCommand joinGameCommand;

		/// <summary>
		/// Sends the typed message to chat.
		/// </summary>
		public ICommand SendChatMessageCommand
		{
			get
			{
				if (sendChatMessageCommand == null) sendChatMessageCommand = new RelayCommand(param => SendChatMessage());
				return sendChatMessageCommand;
			}
		}
		private RelayCommand sendChatMessageCommand;

		/// <summary>
		/// Inserts a piece into the game board
		/// </summary>
		public ICommand PutPieceCommand
		{
			get
			{
				if(putPieceCommand == null)
					putPieceCommand = new RelayCommand(param => PutPiece(int.Parse(param.ToString())));
				return putPieceCommand;
			}
		}
		private RelayCommand putPieceCommand;

		#endregion

		#region [ Properties ]
		
		public string Title
		{
			get
			{
				return title;
			}

			set
			{
				title = value;
				Notify();
			}
		}
		private string title;

		public bool GameVisible
		{
			get
			{
				return gameVisible;
			}
			set
			{
				if (gameVisible != value)
				{
					gameVisible = value;
					Notify();
					RaisePropertyChanged(nameof(IpVisible));
				}
			}
		}
		private bool gameVisible;

		public bool IpVisible => !GameVisible;

		public bool SetupVisible
		{
			get
			{
				return setupVisible;
			}
			set
			{
				setupVisible = value;
				Notify();
			}
		}
		private bool setupVisible;


		public bool ServerSearchVisible
		{
			get
			{
				return serverSearchVisible;
			}
			set
			{
				serverSearchVisible = value;
				Notify();
			}
		}
		private bool serverSearchVisible;

		public Color[][] Pieces
		{
			get
			{
				return pieces;
			}
			set
			{
				if (value.Length != GameWidth)
					throw new ArgumentException($"Length of array has to be {GameWidth}!");
				if (value != pieces)
				{
					pieces = value;
					Notify();
				}
			}
		}
		private Color[][] pieces;
		
		public string ChatInput
		{
			get
			{
				return chatInput;
			}
			set
			{
				chatInput = value;
				Notify();
			}
		}
		private string chatInput;
		
		public string IpAddress
		{
			get
			{
				return ipAdress;
			}
			set
			{
				ipAdress = value;
				Notify();
			}
		}
		private string ipAdress;

		#region [ Name ]

		string name;
		public string Name
		{
			get { return name; }
			set
			{
				if (name != value)
				{
					name = value;
					Title = nameof(Connect4LAN) + " - " + name;
					if (client != null)
						client.Name = name;
					Notify();
				}
			}
		}


		public string OpponentName
		{
			get
			{
				return opponentName;
			}
			set
			{
				opponentName = value;
				Notify();
			}
		}
		private string opponentName;

		#endregion 


		public ObservableCollection<string> ChatMessages
		{
			get
			{
				if (chatMessages == null)
					chatMessages = new ObservableCollection<string>();
				return chatMessages;
			}
			set
			{
				if (chatMessages != value)
				{
					chatMessages = value;
					Notify();
				}
			}
		}
		private ObservableCollection<string> chatMessages;
		private ConcurrentQueue<string> chatMsg;

		#endregion

		#region [ Fields ]

		public const int GameWidth = 7;
		public const int GameHeight = 6;

		private bool yourTurn;
		Color ownColor = Colors.Yellow;
		Color enemyColor = Colors.Red;
		Network.Clientside.Client client;
		Game.Gameboard board;
		Dispatcher dispatcher;

		#endregion

		#region [ Constructor ]

		/// <summary>
		/// Builds a new ViewModel and initializes an empty field.
		/// </summary>
		public GameViewModel()
		{
			Name = "Player 1";
			#if DEBUG
			if(System.Diagnostics.Debugger.IsAttached)
				Name = "THIS. IS. DEBUG!";
			#endif
			InitChat();
			InitBoard();
			InitClient();
			SetIp(client);
			Title = $"Connect4Lan - {Name}";
			GameVisible = false;
			SetupVisible = false;
			ServerSearchVisible = true;
			dispatcher = Dispatcher.CurrentDispatcher;
		}

		/// <summary>
		/// Sets up the chat.
		/// </summary>
		private void InitChat()
		{
			ChatMessages = new ObservableCollection<string>();
			chatMsg = new ConcurrentQueue<string>();
		}

		/// <summary>
		/// Constructs the gameboard and pieces.
		/// </summary>
		private void InitBoard()
		{
			// Init Board
			board = new Game.Gameboard();
			// Init pieces
			Pieces = new Color[7][];
			for (int i = 0; i < GameWidth; i++)
			{
				Pieces[i] = new Color[GameHeight];
				for (int j = 0; j < GameHeight; j++)
				{
					Pieces[i][j] = Colors.AntiqueWhite;
				}
			}
		}

		/// <summary>
		/// Builds the client object and registers handlers to its events.
		/// </summary>
		private void InitClient()
		{
			if (client != null)
				client.Dispose();
			client = new Network.Clientside.Client();
			client.ChatMessageRecieved += ChatMessageReceivedHandler;
			client.ColorChanged += ColorChanged;
			client.ConnectionLost += ConnectionLost;
			client.MovementRecieved += MovementReceived;
			client.PlayerJoined += PlayerJoined;
			client.PlayerNamedChanged += PlayerNameChanged;
			client.Received += MessageToChat;
			client.ServerMessageRecieved += MessageToChat;
			client.GameOver += GameOver;
			client.ConnectedToServer += ConnectedToServer;
			client.GameStarted += GameStarted;
			client.ServerNotFound += ServerNotFound;
			if (Name != null)
				client.Name = Name;
			client.ConnectToDedicatedServer();
		}

		private void SetIp(Client client)
		{
			string ip = "No Ip";

			if (!string.IsNullOrWhiteSpace(client.IP.MapToIPv4().ToString()))
			{
				ip = "Your IP:\t\t" + client.IP.ToString();
				if(!string.IsNullOrWhiteSpace(client.ServerIp))
				{
					ip += "\nServers IP:\t" + client.ServerIp;
				}
			}

			IpAddress = ip;
		}

		#endregion

		#region [ Destructor ]

		public void Dispose()
		{
			if (client != null)
				client.Dispose();
		}

		#endregion

		#region [ Methods ]

		/// <summary>
		/// Host a game on the dedicated server.
		/// </summary>
		private void HostGame()
		{
			client.SendMessage(null, NetworkMessageType.CreateLobby);
			yourTurn = true;
			ownColor = Colors.Yellow;
			enemyColor = Colors.Red;
			SetupVisible = false;
		}
		
		private void JoinGame()
		{
			yourTurn = false;
			ownColor = Colors.Red;
			enemyColor = Colors.Yellow;
			SetupVisible = false;
			Dictionary<string, string> lobbies = client.GetLobbies();

			if(lobbies == null)
			{
				SetupVisible = true;
				MessageBox.Show("No lobbies available");
				return;
			}

			var query = new View.LobbyChoiceBox(lobbies.Values.ToList())
			{
				Owner = Application.Current.MainWindow
			};
			query.ShowDialog();

			// Query aborted
			if (!query.OK)
			{
				ResetGame();
				return;
			}

			int lobbyID = int.Parse(lobbies.Single(l => l.Value.Equals(query.SelectedLobby)).Key);

			client.SendMessage(lobbyID, NetworkMessageType.JoinLobby);
		}

		/// <summary>
		/// Connects to the given IP address.
		/// </summary>
		/// <param name="ip"></param>
		/// <returns>Whether or not the connect worked.</returns>
		private bool Connect(string ip)
		{
			return client.Connect(ip);
		}

		/// <summary>
		/// Sends a chat message to the chat window and the server.
		/// </summary>
		/// <param name="msg"></param>
		private void SendChatMessage()
		{
			WriteChatMessage(Name + ": " + ChatInput);
			if(GameVisible)
				client.SendMessage(ChatInput);
			ChatInput = string.Empty;
		}

		/// <summary>
		/// Writes a message to the chat window.
		/// Should be thread safe.
		/// </summary>
		/// <param name="msg"></param>
		private void WriteChatMessage(string msg)
		{
			dispatcher.Invoke(() => ChatMessages.Add(msg));
		}

		/// <summary>
		/// Puts a piece into the given column and sends it to the server
		/// </summary>
		/// <param name="colIdx">The column to insert the piece into.</param>
		private void PutPiece(int colIdx)
		{
			if (!yourTurn)
			{
				WriteChatMessage("It's not your turn!");
				return;
			}
			int row = int.MaxValue;
			try
			{
				row = board.PutPiece(colIdx, new Game.Piece() { Color = ownColor });
				yourTurn = false;
			}
			catch (InvalidOperationException ex)
			{
				WriteChatMessage(ex.Message);
				ResetGame();
				return;
			}
			client.SendMessage(new Move { Color = ownColor, Column = colIdx }, NetworkMessageType.Move);
			Pieces[colIdx][row] = ownColor;
			Notify(null);
		}

		/// <summary>
		/// Resets the game to its initial state, offering Host and Join options.
		/// </summary>
		private void ResetGame()
		{
			GameVisible = false;
			SetupVisible = false;
			ServerSearchVisible = true;
			InitBoard();
			InitClient();
			RaisePropertyChanged(null);
		}
		
		#endregion

		#region [ EventHandlers ]

		private void ChatMessageReceivedHandler(object sender, string message)
		{
			WriteChatMessage(OpponentName + ": " + message);
		}

		private void ColorChanged(object sender, Color args)
		{
			ownColor = args;
		}

		private void ConnectionLost(object sender, EventArgs args)
		{
			WriteChatMessage("Connection lost");
			ResetGame();
		}

		private void MovementReceived(object sender, Network.Move move)
		{
			if (yourTurn)
			{
				MessageBox.Show("Game out of sync!");
				ResetGame();
			}
			int row = board.PutPiece(move.Column, new Game.Piece() { Color = move.Color });
			Pieces[move.Column][row] = move.Color;
			yourTurn = true;
			RaisePropertyChanged(null);
		}
		
		private void PlayerJoined(object sender, Opponent opponent)
		{
			OpponentName = opponent.Name;
			WriteChatMessage("Your opponent is " + OpponentName);
			GameVisible = true;
		}

		private void PlayerNameChanged(object sender, string newName)
		{
			name = newName;
			Notify("Name");
			Title = $"Connect4Lan - {name}";
		}

		private void MessageToChat(object sender, string msg)
		{
			WriteChatMessage(msg.ToString());
		}

		private void GameOver(object sender, bool iWon)
		{
			string message = iWon ? "You win :)" : "You lose :(";
			WriteChatMessage(message);
			dispatcher.BeginInvoke((Action)(() => MessageBox.Show(Application.Current.MainWindow, message)));
			ResetGame();
		}

		private void ConnectedToServer(object sender, string serverIp)
		{
			SetIp(client);
			Application.Current.Dispatcher.Invoke(() =>
			{
				while(!Application.Current.MainWindow.IsVisible)
					System.Threading.Thread.Sleep(100);
			}
			);
			ServerSearchVisible = false;
			SetupVisible = true;
		}

		private void GameStarted(object sender, EventArgs args)
		{
			GameVisible = true;
		}

		private void ServerNotFound(object sender, string msg)
		{
			client.ConnectToDedicatedServer();
		}

		#endregion

		#region [ NotifyPropertyChanged ]

		private void Notify([CallerMemberName]string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
		{
			Notify(propertyName);
		}
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

	}
}
