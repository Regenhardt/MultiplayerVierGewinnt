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
                }
            }
        }
        private bool gameVisible;

        public bool SetupVisible => !GameVisible;

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
					RaisePropertyChanged("Name");
				}
			}
		}

		#endregion 


		public ObservableCollection<string> ChatMessages
        {
            get
            {
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

        #endregion

        #region [ Fields ]

        public const int GameWidth = 7;
        public const int GameHeight = 6;
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
            InitPieces();
            InitClient();
            Title = $"Connect4Lan - {client.Name}";
            GameVisible = false;
        }

        private void InitPieces()
        {
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
                client.ConnectionLost
                client.MovementRecieved
                client.PlayerJoined
                client.PlayerNamedChanged
                client.Received
                client.ServerMessageRecieved
        }

		#endregion

		#region [ Methods ]
		/// <summary>
		/// Hosts a game and then joins it 
		/// </summary>
		private void HostAndJoinGame()
		{
			server = new Network.Serverside.Server();
			JoinGame("localhost");
		}

		/// <summary>
		/// Queries an IP and connects to it.
		/// </summary>
		/// <param name="ip"></param>
		private void JoinGame()
		{
            var query = new View.QueryBox();
            query.ShowDialog();
			JoinGame(query.IP);
		}

        /// <summary>
        /// Connects to the given IP address.
        /// </summary>
        /// <param name="ip"></param>
        private void JoinGame(string ip)
        {
            client.Connect(ip);
            GameVisible = true;
        }

        private void SendChatMessage(string msg)
		{
			client.SendMessage(msg);
			this.ChatMessages.Add(msg);
		}

        private void PutPiece(int colIdx)
        {
            int row = board.PutPiece(colIdx, new Game.Piece());
        }

        private void ResetGame()
        {
            GameVisible = false;
            InitPieces();
            InitClient();
            RaisePropertyChanged(null);
        }

        #endregion

        #region [ EventHandlers ]

        private void ChatMessageReceivedHandler(object sender, string args)
        {
            ChatMessages.Add(args);
        }

        private void ColorChanged(object sender, Color args)
        {
            ownColor = args;
        }

        private void ConnectionLost(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }

        private void MovementReceived(object sender, Network.Move move)
        {
            board.PutPiece(move.Column, new Game.Piece() { Color = move.Color });
            throw new NotImplementedException("Whos turn? Did someone win?");
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
