using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Connect4LAN.View
{
	/// <summary>
	/// Interaction logic for QueryBox.xaml
	/// </summary>
	public partial class LobbyChoiceBox : Window, INotifyPropertyChanged
	{

		public bool OK;

		public string SelectedLobby => LobbyList.SelectedValue.ToString();

		/// <summary>
		/// Confirm and use the entered IP address.
		/// </summary>
		public ICommand ConfirmLobbyCommand
		{
			get
			{
				if (confirmLobbyCommand == null) confirmLobbyCommand = new RelayCommand(param => ConfirmLobby());
				return confirmLobbyCommand;
			}
		}
		
		private RelayCommand confirmLobbyCommand;
		public LobbyChoiceBox(List<string> lobbies)
		{
			InitializeComponent();
			DataContext = this;
			LobbyList.ItemsSource = lobbies;
			OK = false;
		}

		private void ConfirmLobby()
		{
			OK = true;
			Close();
		}

		private void Notify([CallerMemberName]string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged;	
	}
}
