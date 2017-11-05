using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
#if DEBUG
	/// <summary>
	/// Wrapper for unit tests -> don'T have to declare other classes public
	/// </summary>
	public class Test
	{
		public Test()
		{

		}

		public bool Execute()
		{
			//instantiate and start the server
			Server server = new Server();

			//instaaite a new client and connect him 
			Client client = new Client();
			client.Connect("");


			return true;
		}
	}
#endif
}
