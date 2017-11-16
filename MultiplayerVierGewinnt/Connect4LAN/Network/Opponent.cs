using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4LAN.Network
{
	struct Opponent
	{
		public string Name { get; set; }		

		private string message;

		/// <summary>
		/// The message that is sent 
		/// </summary>
		public Color Color
		{
			get { return (Color)ColorConverter.ConvertFromString(message); }
			set { message = value.ToString(); }
		}
	}
}
