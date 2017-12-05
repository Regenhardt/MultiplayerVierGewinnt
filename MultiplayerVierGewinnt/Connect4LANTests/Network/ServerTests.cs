﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			var client = new UDPBroadcaster(43133);
			client.SendMessage("Hello Broadcast!");

			Assert.IsTrue(true);
		}
	}



}