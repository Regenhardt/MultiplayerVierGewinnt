using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
	class NetworkAdapter : INetworkController
	{
		/// <summary>
		/// Initilizes a Networkadapterr but doesn't instantiate any variables
		/// </summary>
		public NetworkAdapter()
		{

		}
		
		/// <summary>
		/// Connects to the given IP
		/// </summary>
		/// <param name="ip"></param>
		public NetworkAdapter(string ip, int port = 16569)
		{
			Connect(ip, port);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="socket">The connected socket</param>
		/// <exception cref="IOException">When the socket isn't connected</exception>
		public NetworkAdapter(TcpClient socket)
		{
			if (!socket.Connected)
				throw new IOException("The passes TcpClient must be connected. Please use 'new Networkadapter(ip)' instead.");

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

		/// <summary>
		/// THe IPAdress of the connected user
		/// </summary>
		public IPAddress IP { get; private set; }

		/// <summary>
		/// The Socket with wich this player is connected
		/// </summary>
		/// <exception cref="ArgumentNullException"/> 
		public TcpClient Socket
		{
			get { return tcpClient; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();

				if (value != tcpClient)
				{
					tcpClient = value;
					//reset the streams
					@in = new StreamReader(tcpClient.GetStream());
					@out = new StreamWriter(tcpClient.GetStream());
					@out.AutoFlush = true;

					//loop for listenning for incoming messages
					Task.Factory.StartNew(() =>
					{
						//exception is thrown when connection is lost
						try
						{
							while (tcpClient.Connected)
							{
								//read the message and send out the recieved message
								lastMesssage = @in.ReadLine();
								Received?.Invoke(this, lastMesssage);
							}
						}
						catch (NullReferenceException)
						{
							//Object has been disposed -> Closed
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

		//TODO ThreadStatic --> every Thread has its own Copy
		protected TcpClient tcpClient;
		protected StreamReader @in;
		protected StreamWriter @out;
		protected string lastMesssage;

		#endregion

		#region [ Events ]

		public event EventHandler<string> Received;
		public event EventHandler ConnectionLost;

		#endregion

		#region [ INetworkController Members ]

		virtual public bool Connect(string ipAddress, int port = 16569)
		{
			//try to connect tot the socket
			try
			{
				//If socket hasnt been instantiated, instaiate it first
				if (Socket == null)
					Socket = new TcpClient(ipAddress, port);
				else
					//TODO: DIsconnect the old socket and then reconnect ~> Threads overview
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

			//Event of disconnecting will be set in the Thread wich is reading the íncoming messages
		}

		#endregion [ INetworkController Members ]

	}
}
