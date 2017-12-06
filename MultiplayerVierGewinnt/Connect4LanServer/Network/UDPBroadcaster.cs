using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Connect4LanServer.Network
{
	public class UdpBroadcaster : IDisposable
	{
		#region [ Fields ]

		private UdpClient socket;
		private bool listenForMessages = false;
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
		public UdpBroadcaster(int port = 43133, bool startListining = true)
		{
			this.RecievedMessages = new Dictionary<DateTime, string>();
			this.endpoint = new IPEndPoint(IPAddress.Any, port);
			this.socket = new UdpClient(endpoint);
			this.Port = port;

			//start listingi for messages if required
			if (startListining)
				StartListingForMessages();
			
		}

		public void StartListingForMessages()
		{
			if (listenForMessages)
				return;

			listenForMessages = true;
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
		/// Retturns the lcoal IPv4 adress
		/// </summary>
		/// <returns></returns>
		static public IPAddress GetLocalIPAdress()
		{
			return Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork);
		}

		/// <summary>
		/// Checks werther a port is free
		/// </summary>
		/// <param name="port"></param>
		/// <param name="isUdp">True: check UDP, False: check TCP, null: check both</param>
		/// <returns></returns>
		static public bool IsPortFree(int port, bool? isUdp = null)
		{
			IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

			Func<bool> tcpContains = () =>
			{
				TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

				return tcpConnInfoArray.Any(p => p.LocalEndPoint.Port == port) || ipGlobalProperties.GetActiveTcpListeners().Any(p => p.Port == port);
			};

			Func<bool> udpContains = () =>
			{
				var updConInfoArray = ipGlobalProperties.GetActiveUdpListeners();
				return updConInfoArray.Any(p => p.Port == port);
			};

			if(isUdp == null)			
				return !(udpContains() || tcpContains());
			
			return !(isUdp.Value ? udpContains() : tcpContains());
		}

		/// <summary>
		/// Searches the LAN for the running dedicated Gameserver.
		/// Starts with checking localhost
		/// </summary>
		/// <returns>The IPadress of the gameserver</returns>
		/// <exception cref="ServerNotFoundException">When there is no reply from the server for 2.3 seconds</exception>
		public static string FindGameServer()
		{
			if (!IsPortFree(43133))
				return "127.0.0.1";

			try
			{
				using (var client = new UdpBroadcaster(43133, true))
				{
					//as soon as the a message was recieved, please tell meh
					string localIP = GetLocalIPAdress().ToString();
					bool waitingForAnswer = true;
					client.MessageRecieved += (s, e) => { if (localIP != e) waitingForAnswer = false; };
					int tries = 0;
					
					//so we are not the server, broadcast it to everybody
					//tell the server about ourself
					client.SendMessage(localIP);

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
