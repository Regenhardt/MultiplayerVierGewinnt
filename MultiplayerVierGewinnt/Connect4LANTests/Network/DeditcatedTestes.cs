using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect4LanServer;
using Connect4LanServer.Network;
using Connect4LAN.Network;
using System.Threading;

namespace Connect4LANTests.Network
{
	[TestClass()]
	public class DeditcatedTestes
	{
		[TestMethod()]
		public void InstantiateViewmodel()
		{
			//insantiate the viemodel and start the server
			var vm = new DedicatedServerViewModel(new DedicatedServer());
			Thread.Sleep(1143);
			//instantiate a new client
			NetworkAdapter[] arra = new NetworkAdapter[5];
			for (int i = 0; i < 5; i++)
			{
				arra[i] = new NetworkAdapter("127.0.0.5");
				arra[i].SendMessage($"Mar{i}", NetworkMessageType.PlayerName);
				Thread.Sleep(2250);
				arra[i].SendMessage(null, NetworkMessageType.CreateLobby);				
			}

			Thread.Sleep(3000);


			Assert.AreEqual<int>(5, vm.Games.Count);

		}
	}
}
