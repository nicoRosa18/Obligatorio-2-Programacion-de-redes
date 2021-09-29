using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
    public class ServerTools
    {
        private readonly object padlock = new object();
        public bool EndConnection { get; set; }
        public List<Socket> Clients { get; set; }

        public ServerTools()
        {
            EndConnection = false;
            Clients = new List<Socket>();
        }

        public void AddClient(Socket socket)
        {
            lock(padlock)
            {
                Clients.Add(socket);
            }
        }

        public void RemoveClient(Socket socket)
        {
            lock(padlock)
            {
                Clients.Remove(socket);
            }
        }

        public List<Socket> GetClients()
        {
            List<Socket> copyList = new List<Socket>();
            lock(padlock)
            {
                foreach(Socket client in Clients)
                {
                    copyList.Add(client);
                }
            }
            return copyList;
        }
    }
}