using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LanServer
{
	public class DedicatedServerViewModel : INotifyPropertyChanged
	{
		#region [ Properties ]


		public ObservableCollection<Lobby> Games
		{
			get
			{
				return games;
			}
			set
			{
				games = value;
				Notify();
			}
		}
		private ObservableCollection<Lobby> games;

		public int GamesRunning => Games.Count(l => l.State == Lobby.GameState.Running);

		private Network.DedicatedServer Server { get; set; }

		#endregion

		#region [ Constructor ]

		internal DedicatedServerViewModel(Network.DedicatedServer server)
		{
			Server = server;
			Games = new ObservableCollection<Lobby>();
			
		}

		#endregion

		#region [ Eventhandlers ]

		private void OnNewLobbyCreated(object sender, Lobby newLobby)
		{
			Games.Add(newLobby);
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
