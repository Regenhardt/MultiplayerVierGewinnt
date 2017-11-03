using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
	internal interface INetworkController
	{
		
		/// <summary>
		/// EventArgs object contains the received JSON-string
		/// </summary>
		event EventHandler<string> Received;

		/// <summary>
		/// Indicates a critical loss of connection that couldn't be recovered.
		/// </summary>
		event EventHandler ConnectionLost;

		/// <summary>
		/// Connects to the given IP.
		/// </summary>
		/// <param name="ipAddress">The Address to connect to.</param>
		/// <returns>Whether or not the connection was successful.</returns>
		bool Connect(string ipAddress);

		/// <summary>
		/// Returns IP Addresses of available Servers.
		/// </summary>
		/// <returns>IP Addresses of available Servers</returns>
		IEnumerable<string> GetAvailableConnections();

		/// <summary>
		/// Stops the connection.
		/// </summary>
		void Disconnect();
	}
}
