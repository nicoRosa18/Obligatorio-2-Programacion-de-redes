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
            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.5"),30000);
            socketClient.Connect(remoteEndpoint);
            
            Console.WriteLine("Conectado al server remoto, escriba un mensaje, enter para terminar");

            var termine = false;
            while (!termine)
            {
                var message = Console.ReadLine();
                if (message.Length == 0)
                {
                    Console.WriteLine("Cerrando la conexion");
                    termine = true;
                }
                else
                {
                    var data = Encoding.UTF8.GetBytes(message);
                    socketClient.Send(data);
                }
            }
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();
            Console.WriteLine("Cerrando la conexion..");            
            Console.ReadLine();
        }
    }
}
