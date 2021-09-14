using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.Protocol;
using ConsoleAppSocketServer;

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
            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.4"), 30000);
            socketClient.Connect(remoteEndpoint);

            Console.WriteLine("Conectado al server remoto, escriba un mensaje, enter para terminar");

            var termine = false;
            SendMessage(CommandConstants.StartupMenu, socketClient);//pedimos el menu de inicio
            // Si la conexion se cierra, el receive retorna 0
            Console.WriteLine(MessageManager.ShowMessage(socketClient));
            while (!termine)
            {
                string command = Console.ReadLine();
                if (command.Length == 0)
                    {
                        Console.WriteLine("Cerrando la conexion");
                        termine = true;
                    }
                    else
                    {
                        SendMessage(command, socketClient);
                        Console.WriteLine(MessageManager.ShowMessage(socketClient));
                    }
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
        
    }
}
