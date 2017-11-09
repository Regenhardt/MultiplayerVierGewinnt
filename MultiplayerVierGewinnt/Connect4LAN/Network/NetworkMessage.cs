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
	struct NetworkMessage
	{
		/// <summary>
		/// The Type of the Message
		/// </summary>
		public NetworkMessageType MessageType { get; set; }

		private object message;

		/// <summary>
		/// The message that is sent 
		/// </summary>
		public object Message
		{
			get
			{
				if (message is Color)
					return ColorConverter.ConvertFromString(message.ToString());
				else
					return message;
			}
			set
			{
				if (value is Color)
					message = ((Color) value).ToString();
				else
					message = value;
			}
		}


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
