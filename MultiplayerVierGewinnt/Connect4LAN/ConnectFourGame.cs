using Connect4LAN.Network;
using Connect4LAN.Network.Serverside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN
{
    /// <summary>
    /// Represents a Connect4 Game instance
    /// </summary>
    class ConnectFourGame
    {
        public Player player1 { get; private set; }
        public Player player2 { get; private set; }

        /// <summary>
        /// Instaniates a Game of Connect 
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public ConnectFourGame(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }
    }    
}
