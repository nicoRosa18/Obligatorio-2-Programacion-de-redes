using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.Protocol;
// using ConsoleAppSocketServer;

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

            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.10"), 30000);
            socketClient.Connect(remoteEndpoint);

            Console.WriteLine("Conectado al server remoto, escriba un mensaje, enter para terminar");

            bool endConnection = false;
            SendMessage(CommandConstants.StartupMenu, "", socketClient);//pedimos el menu de inicio
 
            Console.WriteLine(ReceiveMessage(socketClient));

            while (!endConnection)
            {
                string message = Console.ReadLine();
                if (message.Equals("exit"))
                {
                    Console.WriteLine("Closing connection");
                    endConnection = true;
                }
                else
                {
                    SendMessage(CommandConstants.Message, message, socketClient);
                    Console.WriteLine(ReceiveMessage(socketClient));
                }
            }
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();        
        }

        private static void SendMessage(int command, string message, Socket socketClient)
        {
            var header = new Header(HeaderConstants.Request, CommandConstants.Message, message.Length);
            var data = header.GetRequest();
            var sentBytes = 0;
            while (sentBytes < data.Length)
            {
                sentBytes += socketClient.Send(data, sentBytes, data.Length - sentBytes, SocketFlags.None);
            }

            sentBytes = 0;
            var bytesMessage = Encoding.UTF8.GetBytes(message);
            while (sentBytes < bytesMessage.Length)
            {
                sentBytes += socketClient.Send(bytesMessage, sentBytes, bytesMessage.Length - sentBytes,
                    SocketFlags.None);
            }
        }

        private static string ReceiveMessage(Socket socketClient)
        {
            int headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                   HeaderConstants.DataLength;
            var buffer = new byte[headerLength];

            ReceiveData(socketClient, headerLength, buffer);

            Header header = new Header();
            header.DecodeData(buffer);            
            var bufferData = new byte[header.IDataLength];  
            ReceiveData(socketClient, header.IDataLength, bufferData);
            
            return Encoding.UTF8.GetString(bufferData);
        }

        private static void ReceiveData(Socket clientSocket,  int length, byte[] buffer)
        {
            var iRecv = 0;
            while (iRecv < length)
            {
                try
                {
                    var localRecv = clientSocket.Receive(buffer, iRecv, length - iRecv, SocketFlags.None);
                    if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                    {
                        throw new Exception("Client is closing");
                    }

                    iRecv += localRecv;
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se.Message);
                    return;
                }
            }
        }
    }
}
