using Microsoft.VisualStudio.TestTools.UnitTesting;
using Connect4LAN.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network.Tests
{
	[TestClass()]
	public class ServerTests
	{
		[TestMethod()]
		public void ServerTest()
		{
			Test test = new Test();
			Assert.IsTrue(test.Execute());
		}
	}
}