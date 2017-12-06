using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace Connect4LAN.Network
{
	public class NetworkAdapter : INetworkController, IDisposable
	{
		#region [ Constuctors ]

		/// <summary>
		/// Initilizes a Networkadapterr but doesn't instantiate any variables
		/// </summary>
		public NetworkAdapter()
		{
				 
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
		/// Connects to the given IP
		/// </summary>
		/// <param name="ip"></param>
		public NetworkAdapter(string ip, int port = 16569)
		{
			Connect(ip, port);
		}

		#endregion [ Constuctors ]

		#region [ Destructors ]

		public void Dispose()
		{
			tcpClient.Close();
		}

		#endregion

		/// <summary>
		/// Setilizes a message and sends it 
		/// </summary>
		/// <param name="msg">The message to be transmitted</param>
		/// <exception cref="ArgumentException"/>
		virtual public void SendMessage(dynamic msg, NetworkMessageType type)
		{
			//TODO: Valuechecking
			try
			{
				switch (type)
				{
					case NetworkMessageType.ServerMessage:
					case NetworkMessageType.ChatMessage:
					case NetworkMessageType.PlayerName:
						@out.WriteLine(new NetworkMessage<string> { Message = msg, MessageType = type }.Serialize());
						break;
					case NetworkMessageType.Color:
						@out.WriteLine(new NetworkMessage<Color> { Message = msg, MessageType = type }.Serialize());
						break;
					case NetworkMessageType.Move:
						@out.WriteLine(new NetworkMessage<Move> { Message = new Move { Color = msg.Color, Column = msg.Column }, MessageType = type }.Serialize());
						break;
					case NetworkMessageType.PlayerConnected:
						@out.WriteLine(new NetworkMessage<Opponent> { Message = msg, MessageType = type }.Serialize());
						break;
					case NetworkMessageType.GameOver:
						@out.WriteLine(new NetworkMessage<bool> { Message = msg, MessageType = type }.Serialize());
						break;

					default:
						throw new ArgumentException();
				}
			}
			catch (IOException)
			{
				ConnectionLost?.Invoke(this, EventArgs.Empty);
			}
			
		}

		/// <summary>
		/// Returns the last message from the player
		/// </summary>
		/// <returns></returns>
		public string ReadLastMessage()
		{
			return lastNetworkMesssage;
		}

		#region  [ Properties ]

		/// <summary>
		/// The IPAdress of the connected user
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
								//report that message was 			
								var msg = @in.ReadLine();
								Received?.Invoke(this, msg);
								lastNetworkMesssage = msg;
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
		protected string lastNetworkMesssage;

		#endregion

		#region [ INetworkController Members ]

		virtual	public event EventHandler<string> Received;
		public event EventHandler ConnectionLost;

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
		/// Closes the Socket (same as Dispose()).
		/// </summary>
		public void Disconnect()
		{
			Socket.Close();
			//Event of disconnecting will be set in the Thread wich is reading the íncoming messages
		}

		#endregion [ INetworkController Members ]

	}
}
