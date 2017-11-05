using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
    /// <summary>
    /// The client wich connects to the host    
    /// </summary>
    class Client : INetworkController
    {
        public event EventHandler ConnectionLost;
        public event EventHandler<string> Received;

		private TcpClient client;
		

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
				if (client != null)
					client.Close();
				client = new TcpClient(ipAddress, port);
				
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
			if(client != null)
				client.Close();
        }
		

		/// <exception cref="NotImplementedException"/>
        public IEnumerable<string> GetAvailableConnections()
        {
            throw new NotImplementedException();
        }
    }
}
