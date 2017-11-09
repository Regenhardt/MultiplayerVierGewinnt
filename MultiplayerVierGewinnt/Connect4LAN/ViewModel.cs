using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
                if (joinGameCommand == null) joinGameCommand = new RelayCommand(param => JoinGame((string) param));
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
        Network.Serverside.Server server;
        Network.Clientside.Client client;

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Builds a new ViewModel and initializes an empty field.
        /// </summary>
        public ViewModel()
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
            client = new Network.Clientside.Client();
            Title = $"Connect4Lan - {client.Name}";
            GameVisible = false;
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
		/// Connects to the given IP
		/// </summary>
		/// <param name="ip"></param>
		private void JoinGame(string ip)
		{
			client.Connect(ip);
		}

		private void SendChatMessage(string msg)
		{
			client.SendMessage(msg);
			this.ChatMessages.Add(msg);
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
