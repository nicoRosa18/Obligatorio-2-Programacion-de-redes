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
            ClientManager manager = new ClientManager();
            manager.Start();
        }
    }
}
