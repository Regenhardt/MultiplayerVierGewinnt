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
    class Player : INetworkController
    {
        public Color Color { get; private set; }
        public string Name { get; private set; }
        public IPAddress IP { get; private set; }

        private TcpClient connectionSocket;
        private StreamReader @in;
        private StreamWriter @out;

        public event EventHandler<string> Received;
        public event EventHandler ConnectionLost;

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="sck"></param>
        /// <exception cref="ArgumentNullException"/>
        public Player(Color color, string name, IPAddress ip, TcpClient sck)
        {
            this.Color = color;
            this.Name = name;
            this.IP = ip;
            this.Socket = sck;
        }

      
        public bool Connect(string ipAddress, int port)
        {
            //try to connect tot the socket
            try
            {
                Socket.Connect(ipAddress, port);
                return Socket.Connected;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<string> GetAvailableConnections()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the Socket
        /// </summary>
        public void Disconnect()
        {
            Socket.Close();
        }
    }
}
