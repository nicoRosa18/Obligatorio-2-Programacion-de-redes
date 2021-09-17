using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Domain;

namespace Server
{
    public class Program
    {
        private static UsersAndCatalogueManager _usersAndCatalogueManager; //sacar
        private static bool _endConnection = false; 
        private static List<Socket> _clients = new List<Socket>();

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

                        foreach (var client in _clients)
                        {
                            Console.WriteLine("un cliente");
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                        }

                        var fakeSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                        var remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.10"), 30000);
                        fakeSocket.Connect(remoteEndpoint);


                        socketServer.Close(); 
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
                    Console.WriteLine($"Nueva coneccion {threadId} aceptada"); //sacar

                    var threadConnection = new Thread(() => HandleConnection(connectedSocket, threadId));
                    threadConnection.Start();
                }
                catch(SocketException se)
                {
                    Console.WriteLine(se);
                    Console.WriteLine("a");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _endConnection = true;
                }
            }
            Console.WriteLine("Cerrando el server...");
        }

        private static void HandleConnection(Socket connectedSocket, int threadId)
        {
            while(!_endConnection){
                try
                {
                    Message spanishMessage = new SpanishMessage();
                    Session session = new Session(connectedSocket, threadId, spanishMessage);

                    while (session.Active && !_endConnection)
                    {
                        session.Listen(connectedSocket);
                    }

                    if(!_endConnection){
                        connectedSocket.Shutdown(SocketShutdown.Both);
                        connectedSocket.Close();
                    }
             
                    Console.WriteLine($"{threadId}: Cerrando coneccion...");
                    return;
                }
                catch(SocketException se) //sacar si no es necesario
                {
                    Console.WriteLine(se);
                    _endConnection = true;
                }
                catch (Exception e) //sacar si no es necesario
                {
                    Console.WriteLine(e);
                    _endConnection = true;
                } 
            }
        }
    }
}
