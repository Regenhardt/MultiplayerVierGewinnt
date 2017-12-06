using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Connect4LanServer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public void OnAppStartup(object sender, StartupEventArgs e)
		{
			var server = new Network.DedicatedServer();
			var viewModel = new DedicatedServerViewModel(server);
			var window = new DedicatedServerView(viewModel);
			window.Show();
		}
		
	}
}
