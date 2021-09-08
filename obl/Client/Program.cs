using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleAppSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Empezando Socket Client...");

            var socketClient = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            //127.0.0.1 es localhost -> solo permite conexiones dentro de la misma maquina
            // RANGO DE PUERTOS 0 - 65535 (RANGO de 1 a 1024 es reservado)
            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.5"), 30000);
            socketClient.Connect(remoteEndpoint);

            Console.WriteLine("Conectado al server remoto, escriba un mensaje, enter para terminar");

            var termine = false;
            SendMessage("startupMenu", socketClient);//pedimos el menu de inicio
            // Si la conexion se cierra, el receive retorna 0
            ShowMessage(socketClient);
            while (!termine)
            {
                string command = Console.ReadLine();
                SendMessage(command, socketClient);
                ShowMessage(socketClient);
                /*    var message = Console.ReadLine();
                    if (message.Length == 0)
                    {
                        Console.WriteLine("Cerrando la conexion");
                        termine = true;
                    }
                    else
                    {
                        var data = Encoding.UTF8.GetBytes(message);
                        socketClient.Send(data);
                    }*/
            }
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();
            Console.WriteLine("Cerrando la conexion..");            
            Console.ReadLine();
        }

        private static void SendMessage(string message, Socket socketClient)
        {
            var bytes = Encoding.UTF8.GetBytes($"{message}*");
            socketClient.Send(bytes);
        }
        private static void ShowMessage(Socket socketClient)
        {
            var buffer = new byte[1024];
            var bytesReceived = 1;
            bytesReceived = socketClient.Receive(buffer);
            if (bytesReceived > 0)
            {
                var message = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(message);
            }
        }
    }
}
