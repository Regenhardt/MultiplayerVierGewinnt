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

namespace Connect4LAN
{
    internal class ViewModel : INotifyPropertyChanged
    {

        #region [ Commands ]

        /// <summary>
        /// Host a game
        /// </summary>
        public ICommand HostGameCommand
        {
            get
            {
                if (hostGameCommand == null) hostGameCommand = new RelayCommand(param => HostAndJoinGame());
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
                if (sendChatMessageCommand == null) sendChatMessageCommand = new RelayCommand(param => SendChatMessage((string) param));
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
                return putPieceCommand?? new RelayCommand(param => PutPiece(int.Parse(param.ToString())));
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
                    RaisePropertyChanged(nameof(SetupVisible));
                }
            }
        }
        private bool gameVisible;

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

		#region [ Name ]

		string name;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (name != value)
				{
					name = value;
                    Title = "Connect4Lan - " + name;
					RaisePropertyChanged("Name");
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
        private ConcurrentBag<string> chatMsg;

        #endregion

        #region [ Fields ]

        public const int GameWidth = 7;
        public const int GameHeight = 6;
        private bool yourTurn;
        Color ownColor = Colors.Yellow;
        Color enemyColor = Colors.Red;
        Network.Serverside.Server server;
        Network.Clientside.Client client;
        Game.Gameboard board;

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Builds a new ViewModel and initializes an empty field.
        /// </summary>
        public ViewModel()
        {
            InitChat();
            InitBoard();
            InitClient();
            Title = $"Connect4Lan - {client.Name}";
            GameVisible = false;
            SetupVisible = true;
        }

        private void InitChat()
        {
            new Thread(RunChat).Start();
        }
        private void RunChat()
        {
            ChatMessages = new ObservableCollection<string>();
            chatMsg = new ConcurrentBag<string>();
            while (true)
            {
                if (chatMsg.TryTake(out string message))
                {
                    ChatMessages.Add(message);
                }
                Thread.Sleep(100);
            }
        }
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

        private void InitClient()
        {
            client = new Network.Clientside.Client();
            client.ChatMessageRecieved += ChatMessageReceivedHandler;
            client.ColorChanged += ColorChanged;
            client.ConnectionLost += ConnectionLost;
            client.MovementRecieved += MovementReceived;
            client.PlayerJoined += PlayerJoined;
            client.PlayerNamedChanged += PlayerNameChanged;
            client.Received += MessageToChat;
            client.ServerMessageRecieved += MessageToChat;
        }

		#endregion

		#region [ Methods ]
		/// <summary>
		/// Hosts a game and then joins it 
		/// </summary>
		private void HostAndJoinGame()
		{
            yourTurn = true;
            ownColor = Colors.Yellow;
            enemyColor = Colors.Red;
            SetupVisible = false;
            Task.Run((Action)HostGame);
		}

        private void HostGame()
        {
            if (server != null) server.Stop();
			server = new Network.Serverside.Server();
            Connect("localhost");
        }


		private void JoinGame()
		{
            yourTurn = false;
            ownColor = Colors.Red;
            enemyColor = Colors.Yellow;
            SetupVisible = false;
            var query = new View.QueryBox();
            query.ShowDialog();
            if (Connect(query.IP))
            {
                GameVisible = true;
            }
            else
            {
                MessageBox.Show("Connection failed!");
                ResetGame();
            }
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
        private void SendChatMessage(string msg)
		{
			client.SendMessage(msg);
            WriteChatMessage(msg);
		}

        private void WriteChatMessage(string msg)
        {
            chatMsg.Add(msg);
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
            yourTurn = false;
            int row = board.PutPiece(colIdx, new Game.Piece() { Color = ownColor });
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
            SetupVisible = true;
            InitBoard();
            InitClient();
            RaisePropertyChanged(null);
        }
        
        #endregion

        #region [ EventHandlers ]

        private void ChatMessageReceivedHandler(object sender, string args)
        {
            ChatMessages.Add(OpponentName + ": " + args);
        }

        private void ColorChanged(object sender, Color args)
        {
            ownColor = args;
        }

        private void ConnectionLost(object sender, EventArgs args)
        {
            ChatMessages.Add("Connection lost");
            ResetGame();
        }

        private void MovementReceived(object sender, Network.Move move)
        {
            if (yourTurn)
            {
                MessageBox.Show("Game out of sync!");
                ResetGame();
            }
            board.PutPiece(move.Column, new Game.Piece() { Color = move.Color });
            yourTurn = true;
            RaisePropertyChanged(null);
        }
        
        private void PlayerJoined(object sender, Opponent opponent)
        {
            OpponentName = opponent.Name;
            ChatMessages.Add("Your opponent is " + OpponentName);
            GameVisible = true;
        }

        private void PlayerNameChanged(object sender, string newName)
        {
            Name = newName;
        }

        private void MessageToChat(object sender, string msg)
        {
            WriteChatMessage(msg.ToString());
        }

        #endregion

        private void Notify([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            Notify(propertyName);
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
