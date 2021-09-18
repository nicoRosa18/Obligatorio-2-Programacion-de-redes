using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.Domain;
using Server;
using Common.Communicator;

namespace Server
{
    public class ServerManager
    {
        private ServerTools _serverAttributes {get; set;}
        private Socket _socketServer {get; set;}
        public ServerManager(Socket socketServer)
        {
            UsersAndCatalogueManager _usersAndCatalogueManager = UsersAndCatalogueManager.Instance;
            Session._usersAndCatalogueManager = _usersAndCatalogueManager;

            _serverAttributes = new ServerTools();
            _socketServer = socketServer;

            StartUpMenu();
            CreateConnections();
        }

        private void CreateConnections(){
            var threadServer = new Thread(()=> ListenForConnections());
            threadServer.Start();

            while (!_serverAttributes.EndConnection)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "exit":
                        
                        _serverAttributes.EndConnection = true;
                        foreach (var client in _serverAttributes.Clients)
                        {
                            Console.WriteLine("un cliente");
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                        }

                        var fakeSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                        var remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.5"), 30000);
                        fakeSocket.Connect(remoteEndpoint);

                        _socketServer.Close(); 
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada, ingrese de nuevo");
                        //session.MessageInterpreter(userInput);
                        break;
                }
            }
        }

        private void ListenForConnections()
        {
            int threadCount = 0;
            while (!_serverAttributes.EndConnection)
            {
                try
                {
                    threadCount++;
                    int threadId = threadCount;

                    var connectedSocket = _socketServer.Accept();
                    _serverAttributes.AddClient(connectedSocket);
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
                    _serverAttributes.EndConnection = true;
                }
            }
            Console.WriteLine("Cerrando el server...");
        }

        private void HandleConnection(Socket connectedSocket, int threadId)
        {
            while(!_serverAttributes.EndConnection){ //probar sacarlo
                try
                {
                    Message spanishMessage = new SpanishMessage();
                    CommunicationSocket communicationViaClient = new CommunicationSocket(connectedSocket);
                    Session session = new Session(communicationViaClient, spanishMessage);

                    while (session.Active && !_serverAttributes.EndConnection)
                    {
                        session.Listen();
                    }

                    if(!_serverAttributes.EndConnection){
                        connectedSocket.Shutdown(SocketShutdown.Both);
                        connectedSocket.Close();
                    }
             
                    Console.WriteLine($"{threadId}: Cerrando coneccion...");
                    return;
                }
                catch(SocketException se) //sacar si no es necesario
                {
                    Console.WriteLine(se);
                    _serverAttributes.EndConnection = true;
                }
                catch (Exception e) //sacar si no es necesario
                {
                    Console.WriteLine(e);
                    _serverAttributes.EndConnection = true;
                } 
            }
        }
        private void StartUpMenu()
        {
            Console.WriteLine("Bienvenido al Sistema Server");
            Console.WriteLine("Ya se estan recibiendo conecciones");
            Console.WriteLine("Inserte: ");
            Console.WriteLine("exit -> Para cerrar todas las conecciones y abandonar el programa");
            Console.WriteLine("Se mostraran las conecciones realizadas");
        }   
    }
}