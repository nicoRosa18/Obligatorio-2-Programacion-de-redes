using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ConsoleAppSocketServer.Domain;

namespace ConsoleAppSocketServer
{
    class Program
    {
        private UsersAndCatalogueManager _usersAndCatalogueManager = new UsersAndCatalogueManager();
        static void Main(string[] args)
        {
            
            Console.WriteLine("Comenzando Socket Server...");

            var socketServer = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            Console.WriteLine("Creado el Socket");
            //127.0.0.1 es localhost -> solo permite conexiones dentro de la misma maquina
            var localEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.5"),30000);
            
            socketServer.Bind(localEndpoint);
            Console.WriteLine("Socket bound to local IP");
            //Luego del listen, ponemos el socket server en modo de aceptar conexiones y ya NO lo 
            // podemos usar para recibir o enviar datos
            socketServer.Listen(10);
            Console.WriteLine("Socket esta en modo escucha");
            var Termine = false;
            var threadCount = 0;
            while (!Termine)
            {
                var connectedSocket = socketServer.Accept();
                threadCount++;
                var threadId = threadCount;
                Console.WriteLine("Acepte una conexion...");
                var threadConnection = new Thread(() => HandleConnection(connectedSocket,threadId));
                threadConnection.Start();
            }
        }

        private static void HandleConnection(Socket connectedSocket, int threadId)
        {
            var bytesReceived = 1;
            // Este while se usa para mantenerse aqui mientras la conexion no se cierra
            while (bytesReceived > 0)
            {
                var buffer = new byte[1024];
                // Si la conexion se cierra, el receive retorna 0
                bytesReceived = connectedSocket.Receive(buffer);
                
                if (bytesReceived > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine(message);
                    var messagecleared = EliminarEspacios(message);
                    MessagesManager.MessageInterpreter(messagecleared, connectedSocket); //interpreta el mensaje recibido y genera una respuesta
                    Console.WriteLine("abajo MessageInterpreteer en program.cs");
                }
                else
                {
                    Console.WriteLine($"{threadId}: El cliente remoto cerro la conexion...");
                }
            }

            // Iniciamos el proceso de cerrado del socket
            connectedSocket.Shutdown(SocketShutdown.Both);
            connectedSocket.Close();
            Console.WriteLine($"{threadId}: Cerrando la  conexion...");
        }

        private static string EliminarEspacios(string message)
        {
            Console.WriteLine("en eliminar espacios");
            int coutAt=message.IndexOf("*");
            string newMessage = message.Substring(0, coutAt);
            Console.WriteLine("termine eliminar espacios");
            return newMessage;
        }
    }
}
