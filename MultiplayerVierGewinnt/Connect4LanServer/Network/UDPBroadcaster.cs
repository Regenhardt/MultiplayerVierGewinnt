using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Connect4LanServer.Network
{
	public class UDPBroadcaster : IDisposable
	{
		#region [ Fields ]

		private UdpClient socket;
		private bool listenForMessages = true;
		private IPEndPoint endpoint;

		#endregion [ Fields ]

		#region [ Properties ]
		/// <summary>
		/// The Port wich is listned and which is sent to
		/// </summary>
		public int Port { get; private set; }

		/// <summary>
		/// Dictionairy of recieved messages
		/// </summary>

		public Dictionary<DateTime, string> recievedMessages { get; private set; }

		#endregion [ Properties ]
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="port"></param>
		public UDPBroadcaster(int port = 43133)
		{
			this.recievedMessages = new Dictionary<DateTime, string>();
			this.endpoint = new IPEndPoint(IPAddress.Any, port);
			this.socket = new UdpClient(endpoint);
			this.Port = port;

			//listen for incoming stuff
			Task.Factory.StartNew(() =>
			{
				while (listenForMessages && socket != null)
				{
					try
					{
						var data = Encoding.UTF8.GetString(socket.Receive(ref endpoint));
						//fire data if any1 is listing
						recievedMessages.Add(DateTime.Now, data);
						this.MessageRecieved?.Invoke(this, data);
						
					}
					catch (Exception)
					{
						//if socket is null then fire disconneted event
						if (socket == null)
							Disconnected?.Invoke(this, EventArgs.Empty);

						System.Diagnostics.Debugger.Break();
					}
				}
			});
		}

		public void Dispose()
		{
			listenForMessages = false;
			Disconnected?.Invoke(this, EventArgs.Empty);
			socket.Dispose();
		}

		#region [ Events and Methods ]
		/// <summary>
		/// Gets fired everytime an event is recived
		/// </summary>
		public event EventHandler<string> MessageRecieved;
		/// <summary>
		/// gets fired when the socket ist disconencted or disposed
		/// </summary>
		public event EventHandler Disconnected;

		/// <summary>
		/// Broadcasts a message
		/// </summary>
		/// <param name="msg"></param>
		public void SendMessage(string msg)
		{
			SendMessage(msg, "255.255.255.255");
		}

		/// <summary>
		/// Sends a message to the designated host
		/// </summary>
		/// <param name="msg"></param>
		public void SendMessage(string msg, string host)
		{
			var bytes = Encoding.UTF8.GetBytes(msg);

			socket.Send(bytes, bytes.Length, host, Port);
		}

		#endregion [ Events and Methods ]
	}
}
