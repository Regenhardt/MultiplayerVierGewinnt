using System.Windows;

namespace Connect4LAN
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window window;
        public void StartApp()
        {
            window = new Connect4LanFactory().GetWindow();
            window.Show();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            StartApp();
            this.DispatcherUnhandledException += UnhandledExceptionHandler;
        }

        private void UnhandledExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(window, e.Exception.Message, "Unhandled Exception");
            System.Environment.Exit(1);
        }
    }
}
