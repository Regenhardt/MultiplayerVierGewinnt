using System;
using System.Runtime.Serialization;

namespace Connect4LAN.Network
{
	[Serializable]
	internal class ServerNotFoundException : Exception
	{
		public ServerNotFoundException()
		{
		}

		public ServerNotFoundException(string message) : base(message)
		{
		}

		public ServerNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}