using Microsoft.VisualStudio.TestTools.UnitTesting;
using Connect4LAN.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect4LanServer.Network;

namespace Connect4LAN.Network.Tests
{
	[TestClass()]
	public class ServerTests
	{
		[TestMethod()]
		public void ServerTest()
		{
			Game.Gameboard gb = new Game.Gameboard();
			for (int i = 0; i < gb.Board.GetLength(0); i++)
			{
				gb.Board[i, gb.Board.GetLength(1) - 1] = new Game.Piece();
			}

			Assert.IsTrue(gb.IsFull);
		}

		[TestMethod()]
		public void BroadcastTest()
		{
			//instantiate a port
			var client = new UdpBroadcaster(43133);
			client.SendMessage("Hello Broadcast!");
			client.Dispose();

			Assert.IsTrue(true);
		}

		[TestMethod()]
		public void BroadcastRecieveTest()
		{
			//instantiate a port
			using (var client = new UdpBroadcaster(43133))
			{
				var c = UdpBroadcaster.FindGameServer();
				Assert.AreEqual("127.0.0.1", c);
			}
		}

		[TestMethod()]
		public void DedicatedTest()
		{
			//instantiate a port
			var boss = new DedicatedServer();			
			var c = UdpBroadcaster.FindGameServer();
			while (true) ;
#pragma warning disable CS0162 // Unreachable code detected
			Assert.AreEqual("127.0.0.1", c);
#pragma warning restore CS0162 // Unreachable code detected

		}


		[TestMethod]
		public void PortFree()
		{
			Assert.IsTrue(UdpBroadcaster.IsPortFree(43133));
			UdpBroadcaster ddd = new UdpBroadcaster();
			Assert.IsFalse(UdpBroadcaster.IsPortFree(43133));

		}

		[TestMethod]
		public void FindServerTest()
		{
			var i = UdpBroadcaster.FindGameServer();
			System.Net.IPAddress a;
			// Is an IP bAdress
			Assert.IsTrue(System.Net.IPAddress.TryParse(i, out a));

			// Isn't localhost
			Assert.AreNotEqual(i, UdpBroadcaster.GetLocalIPAdress().MapToIPv4().ToString());
		}

	}



}