using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
	struct Chatmessage
	{
		public string Message { get; set; }
		public string From { get; set; }
		public DateTime Sent { get; set; }
	

		public Chatmessage(string msg, string from)
		{
			this.Message = msg;
			this.From = from;

			Sent = DateTime.Now;
		}
	}
}
