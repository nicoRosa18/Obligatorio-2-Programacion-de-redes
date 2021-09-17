using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Domain;
using Server;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {            
            UsersAndCatalogueManager _usersAndCatalogueManager = UsersAndCatalogueManager.Instance;
            Session._usersAndCatalogueManager = _usersAndCatalogueManager;

            ServerTools serverAttributes = new ServerTools();

            Console.WriteLine("Comenzando Socket Server...");

            var socketServer = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            socketServer.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.10"),30000));
            socketServer.Listen(10);

            var threadServer = new Thread(()=> ListenForConnections(socketServer, serverAttributes));
            threadServer.Start();

            Console.WriteLine("Bienvenido al Sistema Server");
            Console.WriteLine("Ya se estan recibiendo conecciones");
            Console.WriteLine("Inserte: ");
            Console.WriteLine("exit -> Para cerrar todas las conecciones y abandonar el programa");
            Console.WriteLine("Se mostraran las conecciones realizadas");
            while (!serverAttributes.EndConnection)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "exit":
                        serverAttributes.EndConnection = true;

                        foreach (var client in serverAttributes.Clients)
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

        private static void ListenForConnections(Socket socketServer, ServerTools serverAttributes)
        {
            int threadCount = 0;
            while (!serverAttributes.EndConnection)
            {
                try
                {
                    threadCount++;
                    int threadId = threadCount;

                    var connectedSocket = socketServer.Accept();
                    serverAttributes.AddClient(connectedSocket);
                    Console.WriteLine($"Nueva coneccion {threadId} aceptada"); //sacar

                    var threadConnection = new Thread(() => HandleConnection(connectedSocket, threadId, serverAttributes));
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
                    serverAttributes.EndConnection = true;
                }
            }
            Console.WriteLine("Cerrando el server...");
        }

        private static void HandleConnection(Socket connectedSocket, int threadId, ServerTools serverAttributes)
        {
            while(!serverAttributes.EndConnection){
                try
                {
                    Message spanishMessage = new SpanishMessage();
                    Session session = new Session(connectedSocket, threadId, spanishMessage);

                    while (session.Active && !serverAttributes.EndConnection)
                    {
                        session.Listen(connectedSocket);
                    }

                    if(!serverAttributes.EndConnection){
                        connectedSocket.Shutdown(SocketShutdown.Both);
                        connectedSocket.Close();
                    }
             
                    Console.WriteLine($"{threadId}: Cerrando coneccion...");
                    return;
                }
                catch(SocketException se) //sacar si no es necesario
                {
                    Console.WriteLine(se);
                    serverAttributes.EndConnection = true;
                }
                catch (Exception e) //sacar si no es necesario
                {
                    Console.WriteLine(e);
                    serverAttributes.EndConnection = true;
                } 
            }
        }
    }
}
