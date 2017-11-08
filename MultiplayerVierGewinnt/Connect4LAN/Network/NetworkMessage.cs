using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Connect4LAN.Network
{
	/// <summary>
	/// A Message deliverd across the network
	/// </summary>
	struct NetworkMessage
	{
		/// <summary>
		/// The Type of the Message
		/// </summary>
		public NetworkMessageType MessageType { get; set; }
		
		/// <summary>
		/// The message that is sent
		/// </summary>
		public object Message { get; set; }

		/// <summary>
		/// wrapper for ToString() -> Serilizes the object
		/// </summary>
		/// <returns></returns>
		public string Serilize() => ToString();

		/// <summary>
		/// Serilizes the object
		/// </summary>
		/// <returns></returns>
		public override string ToString() => new JavaScriptSerializer().Serialize(this);
		

		/// <summary>
		/// Deseitilizes a Networkmessage
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static NetworkMessage DeSerilize(string s) => new JavaScriptSerializer().Deserialize<NetworkMessage>(s);
	}
}
