using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.Domain;

namespace ConsoleAppSocketServer
{
    class Program
    {
        private static UsersAndCatalogueManager _usersAndCatalogueManager;
        static bool _endConnection = false;
        static List<Socket> _clients = new List<Socket>();

        static void Main(string[] args)
        {
            _usersAndCatalogueManager = new UsersAndCatalogueManager(); 
            Session._usersAndCatalogueManager = _usersAndCatalogueManager;

            Console.WriteLine("Comenzando Socket Server...");

            var socketServer = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            socketServer.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.10"),30000));
            socketServer.Listen(10);

            var threadServer = new Thread(()=> ListenForConnections(socketServer));
            threadServer.Start();

            Console.WriteLine("Bienvenido al Sistema Server");
            Console.WriteLine("Ya se estan recibiendo conecciones");
            Console.WriteLine("Inserte: ");
            Console.WriteLine("exit -> Para cerrar todas las conecciones y abandonar el programa");
            Console.WriteLine("Se mostraran las conecciones realizadas");
            while (!_endConnection)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "exit":
                        _endConnection = true;
                        socketServer.Close(0);
                        foreach (var client in _clients)
                        {
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                        }
                        Console.WriteLine("a");
                        var fakeSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                        fakeSocket.Connect("192.168.1.10",30000);
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada, ingrese de nuevo");
                        break;
                }
            }
        }

        private static void ListenForConnections(Socket socketServer)
        {
            int threadCount = 0;
            while (!_endConnection)
            {
                try
                {
                    threadCount++;
                    int threadId = threadCount;

                    var connectedSocket = socketServer.Accept();
                    _clients.Add(connectedSocket);
                    Console.WriteLine($" Nueva coneccion {threadId} aceptada");

                    var threadConnection = new Thread(() => HandleConnection(connectedSocket, threadId));
                    threadConnection.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // _endConnection = true;
                }
            }
            Console.WriteLine("Exiting....");
        }

        private static void HandleConnection(Socket connectedSocket, int threadId)
        {
            Message spanishMessage = new SpanishMessage();
            Session session = new Session(connectedSocket, threadId, spanishMessage);

            while (session.Active)
            {
                session.Listen(connectedSocket);
            }

            // Iniciamos el proceso de cerrado del socket
            connectedSocket.Shutdown(SocketShutdown.Both);
            connectedSocket.Close();
            Console.WriteLine($"{threadId}: Cerrando coneccion...");
        }
    }
}
