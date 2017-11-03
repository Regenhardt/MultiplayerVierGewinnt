using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4LAN.Network
{
    /// <summary>
    /// The client wich connects to the host
    /// </summary>
    class Client : INetworkController
    {
        public event EventHandler ConnectionLost;
        public event EventHandler<string> Received;

        public bool Connect(string ipAddress)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAvailableConnections()
        {
            throw new NotImplementedException();
        }
    }
}
