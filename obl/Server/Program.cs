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
        private static UsersAndCatalogueManager _usersAndCatalogueManager;

        static void Main(string[] args)
        {
            _usersAndCatalogueManager = new UsersAndCatalogueManager(); 
            Session._usersAndCatalogueManager = _usersAndCatalogueManager;

            Console.WriteLine("Comenzando Socket Server...");

            var socketServer = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            Console.WriteLine("Creado el Socket");

            //127.0.0.1 es localhost -> solo permite conexiones dentro de la misma maquina
            socketServer.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.5"),30000));
            Console.WriteLine("Socket bindeado a la IP local");

            //Luego del listen, ponemos el socket server en modo de aceptar conexiones y ya NO lo 
            // podemos usar para recibir o enviar datos
            socketServer.Listen(10);
            Console.WriteLine("Socket esta en modo escucha");

            bool endConnection = false;
            int threadCount = 0;
            while (!endConnection)
            {
                var connectedSocket = socketServer.Accept();
                threadCount++;
                int threadId = threadCount;
                Console.WriteLine("Acepte una conexion...");
                var threadConnection = new Thread(() => HandleConnection(connectedSocket,threadId));
                threadConnection.Start();
            }
        }

        private static void HandleConnection(Socket connectedSocket, int threadId)
        {
            Message spanishMessage = new SpanishMessage();
            Session session = new Session(connectedSocket,threadId, spanishMessage);

            while (session.Active)
            {
                session.Listen(connectedSocket);
            }

            // Iniciamos el proceso de cerrado del socket
            connectedSocket.Shutdown(SocketShutdown.Both);
            connectedSocket.Close();
            Console.WriteLine($"{threadId}: Closing connection...");
        }

       
    }
}
