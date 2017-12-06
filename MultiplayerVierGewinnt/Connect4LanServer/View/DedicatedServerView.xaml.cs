using System.Windows;

namespace Connect4LanServer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class DedicatedServerView : Window
	{
		public DedicatedServerView(object datacontext)
		{
			this.DataContext = datacontext;
			InitializeComponent();
		}
	}
}
