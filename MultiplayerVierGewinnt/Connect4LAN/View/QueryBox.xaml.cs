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
    public partial class QueryBox : Window, INotifyPropertyChanged
    {

        public string IP
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
                Notify();
            }
        }
        private string ip;

        /// <summary>
        /// Confirm and use the entered IP address.
        /// </summary>
        public ICommand ConfirmIPCommand
        {
            get
            {
                if (confirmIPCommand == null) confirmIPCommand = new RelayCommand(param => ConfirmIP());
                return confirmIPCommand;
            }
        }
        private RelayCommand confirmIPCommand;
        public QueryBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ConfirmIP() => Close();

        private void Notify([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
