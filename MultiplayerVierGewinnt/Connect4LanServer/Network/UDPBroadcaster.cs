using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Connect4LanServer.Network
{
	public class UdpBroadcaster : IDisposable
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

		public Dictionary<DateTime, string> RecievedMessages { get; private set; }

		#endregion [ Properties ]
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="port"></param>
		public UdpBroadcaster(int port = 43133)
		{
			this.RecievedMessages = new Dictionary<DateTime, string>();
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
						System.Diagnostics.Debug.WriteLine("Bevor recieved: " + this.endpoint);
						var data = Encoding.UTF8.GetString(socket.Receive(ref endpoint));
						System.Diagnostics.Debug.WriteLine("After recieved: " + this.endpoint);
						//fire data if any1 is listing
						RecievedMessages.Add(DateTime.Now, data);
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

		/// <summary>
		/// Searches the LAN for the running dedicated Gameserver.
		/// Starts with checking localhost
		/// </summary>
		/// <returns>The IPadress of the gameserver</returns>
		/// <exception cref="ServerNotFoundException">When there is no reply from the server for 2.3 seconds</exception>
		public static string FindGameServer()
		{
			try
			{
				using (var client = new UdpBroadcaster())
				{
					//as soon as the a message was recieved, please tell meh
					bool waitingForAnswer = true;
					client.MessageRecieved += (s, e) => waitingForAnswer = false;
					int tries = 0;

					//TODO: Alternitevly check for openports 
					//tell ourself about ourself
					client.SendMessage("127.0.0.3", "127.0.0.3");
					//give the computer 1 second to react
					while (waitingForAnswer && tries++ < 50)
						System.Threading.Thread.Sleep(20);

					//check if a message was recieved
					if (!waitingForAnswer)
						return "127.0.0.1";

					//so we are not the server, broadcast it to everybody
					//tell the server about ourself
					client.SendMessage(IPAddress.Any.MapToIPv4().ToString());

					//wait for 2.3 seconds for a replay
					tries = 0;
					while (waitingForAnswer && tries++ < 70)
						System.Threading.Thread.Sleep(33);

					//check if we are still waiting for an answer
					if (waitingForAnswer)
						throw new ServerNotFoundException("No response recieved for 2.3 seconds.");
					else
						return client.RecievedMessages.Last().Value;
				}

			}
			catch (SocketException)
			{
				return "127.0.0.1";
			}
		}
		#endregion [ Events and Methods ]
	}
}
