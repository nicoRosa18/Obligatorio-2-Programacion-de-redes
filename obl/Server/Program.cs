using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Domain;
using Server;
using Common.Communicator;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {            
            Console.WriteLine("Comenzando Socket Server...");

            var socketServer = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            socketServer.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.10"),30000));
            socketServer.Listen(10);

            ServerManager serverManager = new ServerManager(socketServer);
        }
    }
}
