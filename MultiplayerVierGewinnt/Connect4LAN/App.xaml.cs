using System.Windows;

namespace Connect4LAN
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new Connect4LanFactory().GetWindow().Show();
        }
    }
}
