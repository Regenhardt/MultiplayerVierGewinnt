using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN.Network
{
    /// <summary>
    /// Class representing a Player
    /// </summary>
    class Player
    {
        public Color Color { get; private set; }
        public string Name { get; private set; }
        public IPAddress IP { get; private set; }

        public Player()
        {
            
        }
    }
}
