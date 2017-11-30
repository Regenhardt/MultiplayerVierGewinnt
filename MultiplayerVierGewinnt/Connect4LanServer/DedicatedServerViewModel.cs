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


		public ObservableCollection<object> Games
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
		private ObservableCollection<object> games;

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
