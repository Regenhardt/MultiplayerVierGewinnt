using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace Connect4LAN.Network
{
	/// <summary>
	/// A Message deliverd across the network
	/// </summary>
	public struct NetworkMessage <T>
	{
		/// <summary>
		/// The Type of the Message
		/// </summary>
		public NetworkMessageType MessageType { get; set; }


		/// <summary>
		/// The message that is sent	
		/// </summary>
		public T Message { get; set; }


		/// <summary>
		/// wrapper for ToString() -> Serilizes the object
		/// </summary>
		/// <returns></returns>
		public string Serialize() => ToString();

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
		public static NetworkMessage<T> DeSerialize(string s) => new JavaScriptSerializer().Deserialize<NetworkMessage<T>>(s);

		/// <summary>
		/// Reads out the type of the NetworkMessage
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static Type GetTypeFromNetworkMessage(string s) => new JavaScriptSerializer().Deserialize<NetworkMessage<object>>(s).MessageType.ToType();
	}
}
