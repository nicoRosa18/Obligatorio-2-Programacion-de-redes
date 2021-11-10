using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.Domain;
using Common.Communicator;
using Common.SettingsManager;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.MessageQueue;

namespace Server
{
    public class ServerManager
    {
        private ServerTools _serverAttributes { get; set; }
        private TcpListener _tcpListener { get; set; }
        private string _serverIpAddress { get; set; }
        private string _serverPort { get; set; }        

        public ServerManager()
        {
            Task server = ServerManagerAsync();
        }

        public async Task ServerManagerAsync()
        {
            UsersAndCatalogueManager usersAndCatalogueManager = UsersAndCatalogueManager.Instance;
            Session._usersAndCatalogueManager = usersAndCatalogueManager;
            LocalSender localSender = LocalSender.Instance;
            Session._localSender = localSender;
            
            _serverAttributes = new ServerTools();

            ISettingsManager _ipConfiguration = new AddressIPConfiguration();
            _serverIpAddress = _ipConfiguration.ReadSetting("ServerIpAddress");
            _serverPort = _ipConfiguration.ReadSetting("ServerPort");

            ISettingsManager _serverConfiguration = new PathsConfiguration();
            System.IO.Directory.CreateDirectory(_serverConfiguration.ReadSetting("CoversPath"));

            _tcpListener  = new TcpListener(new IPEndPoint(IPAddress.Parse(_serverIpAddress), int.Parse(_serverPort)));
            _tcpListener.Start(10);

            StartUpMenu();
            await CreateConnectionsAsync();
        }

        private async Task CreateConnectionsAsync()
        {
            Task task = Task.Run(async () => await ListenForConnectionsAsync().ConfigureAwait(false));

            while (!_serverAttributes.EndConnection)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "exit":
                        _serverAttributes.EndConnection = true;

                        List<TcpClient> clientTcpListenerList = _serverAttributes.GetClients();
                        foreach (var client in clientTcpListenerList)
                        {
                            client.GetStream().Close();
                            client.Close();
                        }

                        TcpClient fakeTcp  = new TcpClient(new IPEndPoint(IPAddress.Parse(_serverIpAddress), 0));
                        await fakeTcp.ConnectAsync(IPAddress.Parse(_serverIpAddress), int.Parse(_serverPort));
                        
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada, ingrese de nuevo");
                        break;
                }
            }
            Task.WaitAll();
        }

        private async Task ListenForConnectionsAsync()
        {
            int threadCount = 0;
            while (!_serverAttributes.EndConnection)
            {
                threadCount++;
                int threadId = threadCount;

                TcpClient connectedTcpClient = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

                _serverAttributes.AddClient(connectedTcpClient);
                Console.WriteLine($"Nueva coneccion {threadId} aceptada");

                Task task = Task.Run(async () => await HandleConnection(connectedTcpClient, threadId).ConfigureAwait(false));
            }

            _tcpListener.Stop();
            Console.WriteLine("Cerrando el server...");
        }

        private async Task HandleConnection(TcpClient connectedTcpClient, int threadId)
        {
            Message spanishMessage = new SpanishMessage();
            CommunicationTcp communicationViaClient = new CommunicationTcp(connectedTcpClient);
            Session session = new Session(communicationViaClient, spanishMessage);

            while (session.Active && !_serverAttributes.EndConnection)
            {
                session.Listen();
            }

            session.LogOut();

            if (!_serverAttributes.EndConnection)
            {
                connectedTcpClient.Close();
                _serverAttributes.RemoveClient(connectedTcpClient);
            }

            Console.WriteLine($"{threadId}: Cerrando coneccion...");
            return;
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