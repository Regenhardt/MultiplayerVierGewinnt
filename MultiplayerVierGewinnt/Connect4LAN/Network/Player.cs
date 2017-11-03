using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        private TcpClient connectionSocket;
        private StreamReader @in;
        private StreamWriter @out;

        /// <summary>
        /// The Socket with wich this player is connected
        /// </summary>
        /// <exception cref="ArgumentNullException"/> 
        public TcpClient Socket
        {
            get { return connectionSocket; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (value != connectionSocket)
                {
                    connectionSocket = value;
                    //reset the streams
                    @in = new StreamReader(connectionSocket.GetStream());
                    @out = new StreamWriter(connectionSocket.GetStream());
                }
            }
        }


        public Player()
        {
            
        }
    }
}
