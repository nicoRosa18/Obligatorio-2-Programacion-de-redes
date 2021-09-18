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
            Console.WriteLine("Empezando Socket Client...");

            var socketClient = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.4"), 30000);
            socketClient.Connect(remoteEndpoint);

            Console.WriteLine("Conectado al server remoto, escriba un mensaje, enter para terminar");
            CommunicationSocket communication = new CommunicationSocket(socketClient);
            bool endConnection = false;

            communication.SendMessage(CommandConstants.StartupMenu, "");//pedimos el menu de inicio
 
            Console.WriteLine(communication.ReceiveMessage().Message);

            while (!endConnection)
            {
                string message = Console.ReadLine();
                //string message = Console.ReadLine(); -> for actual message
                if (message.Equals("exit"))
                {
                    Console.WriteLine("Closing connection");
                    endConnection = true;
                }
                else
                {
                    communication.SendMessage(CommandConstants.Message, message);
                    Console.WriteLine(communication.ReceiveMessage());
                }
            }
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();        
        }
    }
}
