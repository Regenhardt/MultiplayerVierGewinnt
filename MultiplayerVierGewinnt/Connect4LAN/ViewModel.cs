using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public const int GameWidth = 7;
        public const int GameHeight = 6;

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

        public ViewModel()
        {
            Pieces = new Color[7][];
            for(int i = 0; i < GameWidth; i++)
            {
                Pieces[i] = new Color[GameHeight];
            }
        }

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
