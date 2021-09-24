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
            ServerManager serverManager = new ServerManager();
        }
    }
}