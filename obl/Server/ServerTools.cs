using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class ServerTools
    {
        public bool EndConnection {get; set;}
        public List<Socket> Clients {get; set;}

        public ServerTools(){
            EndConnection = false;
            Clients = new List<Socket>();
        }

        public void AddClient(Socket socket)
        {
            Clients.Add(socket);
        }

        public void RemoveClient(Socket socket)
        {
            Clients.Remove(socket);
        }
    }
}