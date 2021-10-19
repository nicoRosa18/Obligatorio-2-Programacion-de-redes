using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
    public class ServerTools
    {
        private readonly object padlock = new object();
        public bool EndConnection { get; set; }
        public List<TcpClient> Clients { get; set; }

        public ServerTools()
        {
            EndConnection = false;
            Clients = new List<TcpClient>();
        }

        public void AddClient(TcpClient tcpClient)
        {
            lock(padlock)
            {
                Clients.Add(tcpClient);
            }
        }

        public void RemoveClient(TcpClient tcpClient)
        {
            lock(padlock)
            {
                Clients.Remove(tcpClient);
            }
        }

        public List<TcpClient> GetClients()
        {
            List<TcpClient> copyList = new List<TcpClient>();
            lock(padlock)
            {
                foreach(TcpClient client in Clients)
                {
                    copyList.Add(client);
                }
            }
            return copyList;
        }
    }
}