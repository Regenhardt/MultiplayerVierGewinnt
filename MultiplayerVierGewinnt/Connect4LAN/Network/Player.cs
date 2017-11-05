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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color">The Color of the Player</param>
		/// <param name="name">The name of the Player</param>
		/// <param name="ip">The IP with wich he is connected</param>
		/// <param name="socket">The Socket on wich e is connected</param>
		/// <exception cref="ArgumentNullException"/>
		public Player(Color color, string name, IPAddress ip, TcpClient socket)
		{
			this.Color = color;
			this.Name = name;
			this.IP = ip;
			this.Socket = socket;
		}

		/// <summary>
		/// Sends a Message to the player
		/// </summary>
		/// <param name="msg"></param>
		public void SendMessage(string msg)
		{
			@out.WriteLine(msg);
		}

		/// <summary>
		/// Returns the last message from the player
		/// </summary>
		/// <returns></returns>
		public string ReadLastMessage()
		{
			return lastMesssage;
		}


		#region  [ Properties ]

		public Color Color { get; private set; }
		public string Name { get; private set; }
		public IPAddress IP { get; private set; }

		
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
					@out.AutoFlush = true;

					//loop for listenning for incoming messages
					Task.Factory.StartNew(() =>
					{
						//exception is thrown when connection is lost
						try
						{
							while(connectionSocket.Connected)
							{
								//read the message and send out the recieved message
								lastMesssage = @in.ReadLine();
								Received?.Invoke(this, lastMesssage);
							}
						}
						catch (IOException)
						{
							//Person disconnected
							ConnectionLost?.Invoke(this, EventArgs.Empty);
						}
						
					});
                }
            }
        }


		#endregion

		#region [ Fields ]

		private TcpClient connectionSocket;
		private StreamReader @in;
		private StreamWriter @out;
		private string lastMesssage;

		#endregion

		#region [ Events ]

		public event EventHandler<string> Received;
		public event EventHandler ConnectionLost;

		#endregion

		#region [ INetworkController Members ]

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

		#endregion [ INetworkController Members ]

	}
}
