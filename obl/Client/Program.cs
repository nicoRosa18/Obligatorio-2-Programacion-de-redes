using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.Protocol;
using Common.Communicator;

namespace Client
{
    public class Program
    {

        static void Main(string[] args)
        {
            var socketClient = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
                
            ClientManager manager = new ClientManager(socketClient);
            manager.Start();
        }
    }
}
