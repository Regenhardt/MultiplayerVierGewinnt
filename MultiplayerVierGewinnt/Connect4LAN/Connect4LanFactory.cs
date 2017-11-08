using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Connect4LAN
{
    public class Connect4LanFactory
    {
        public Connect4LanFactory()
        {
        }
    
        public Window GetWindow()
        {
            var viewModel = new ViewModel();
            var window = new View.GameWindow(viewModel);

            return window;
        }
    }
}
