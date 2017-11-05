using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace Connect4LAN.Network
{
    /// <summary>
    /// The client wich connects to the host    
	/// 
	/// Is the User for the User
    /// </summary>
    class Client : INetworkController
    {
        public event EventHandler ConnectionLost;
        public event EventHandler<string> Received;


		private TcpClient client;
		private StreamReader @in;
		private StreamWriter @out;

		private TcpClient tcpClient
		{
			get { return client; }
			set
			{
				if(value != client)
				{
					client = value;
					
					//reset streams
					if(value != null)
					{
						//reset the streams
						@in = new StreamReader(client.GetStream());
						@out = new StreamWriter(client.GetStream());
						@out.AutoFlush = true;
					}
				}

			}
		}


		/// <summary>
		/// The Name of the player
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The color of this player, set by the server
		/// </summary>
		public Color Color { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the Player</param>
		public Client(string name = "Player1")
		{
			Name = name;
		}

		/// <summary>
		/// Connects to designated IP Adresses.
		/// Catchhes IO Exceptions and returns false in that case. Else it throws the error
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool Connect(string ipAddress, int port = 16569)
        {
			try
			{
				//close current connection and reset the connection
				if (tcpClient != null)
					tcpClient.Close();
				tcpClient = new TcpClient(ipAddress, port);

				//tell the server our Name
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				@out.WriteLine(serializer.Serialize(new { Name = this.Name }));

				//await his answer
				string answer = @in.ReadLine();
				//reset name and Answer
			    this.Name = ((dynamic) serializer.DeserializeObject(answer))["Name"];
				this.Color = ColorConverter.ConvertFromString(((dynamic)serializer.DeserializeObject(answer))["Color"]);				
				

				return true;
			}
			catch (IOException)
			{
				return false;
			}
			catch (Exception)
			{
				throw;
			}
        }

        public void Disconnect()
        {
			if(tcpClient != null)
				tcpClient.Close();
        }
		

		/// <exception cref="NotImplementedException"/>
        public IEnumerable<string> GetAvailableConnections()
        {
            throw new NotImplementedException();
        }
    }
}
