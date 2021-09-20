using System;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Common.Communicator;
using Common.Protocol;
using Common.SettingsManager;

namespace Client
{
    public class ClientManager
    {

        private Socket _socket { get; set; }
        private IPEndPoint _remoteEndpoint { get; set; }
        private CommunicationSocket _communication { get; set; }
        private Message _message { get; set; }
        private string _serverIpAddress { get; set; }
        private string _serverPort { get; set; }

        public ClientManager(Socket socketClient)
        {
            this._message = new SpanishMessage();
            this._socket = socketClient;

            ISettingsManager _ipConfiguration = new AddressIPConfiguration();
            _serverIpAddress = _ipConfiguration.ReadSetting("ServerIpAddress");
            _serverPort = _ipConfiguration.ReadSetting("ServerPort");

            _remoteEndpoint = new IPEndPoint(IPAddress.Parse(_serverIpAddress), int.Parse(_serverPort));

            this._communication = new CommunicationSocket(_socket);   
        }

        public void Start()
        {
            _socket.Connect(_remoteEndpoint);
            Menu();
        }


        private void Menu()
        {
            Console.WriteLine("Conectado al server remoto, escriba un mensaje, enter para terminar");
            CommunicationSocket communication = new CommunicationSocket(_socket);
            bool endConnection = false;
            communication.SendMessage(CommandConstants.StartupMenu, ""); //pedimos el menu de inicio
            Console.WriteLine(communication.ReceiveMessage().Message);
            while (!endConnection)
            {
                string message = Console.ReadLine();
                //string message = Console.ReadLine(); -> for actual message
                if (message.Equals("exit"))
                {
                    Console.WriteLine("Closing connection");
                    endConnection = true;
                }
                else
                {
                    MessageInterpreter(message);
                }
            }

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        private void MessageInterpreter(string message)
        {
            string messageReturn = "";
            try
            {
                switch (Int32.Parse(message))
                {
                    case CommandConstants.StartupMenu:
                        StartUpMenu();
                        break;
                    case CommandConstants.RegisterUser:
                        UserRegistration();
                        break;
                    case CommandConstants.LoginUser:
                        UserLogin();
                        break;
                    case CommandConstants.MainMenu:
                        MainMenu();
                        break;
                    case CommandConstants.AddGame:
                        AddGame();
                        break;
                    case CommandConstants.SearchGame:
                        SearchGame();
                        break;
                    case CommandConstants.MyGames:
                        ShowMyGames();
                        break;
                    case CommandConstants.GameDetails:
                        ShowGameDetails();
                        break;
                    case CommandConstants.buyGame:
                        BuyGame();
                        break;
                    case CommandConstants.PublishQualification:
                        PublishQualification();
                        break;
                    default:
                        messageReturn = "por favor envie una opcion correcta";
                        Console.WriteLine(messageReturn);
                        break;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Equals("exit")) CloseConnection();
                else
                {
                    messageReturn = "por favor envie una opcion correcta";
                    Console.WriteLine(messageReturn);
                }
            }
        }

        private void PublishQualification()
        {
            Console.WriteLine(_message.PublishQualification);
            bool gameNameOk = false;
            string gameName = " ";
            while (!gameNameOk)
            {
                gameName = Console.ReadLine();
                _communication.SendMessage(CommandConstants.GameExists,gameName);
                int res = _communication.ReceiveMessage().Command;
                if (res == CommandConstants.GameExists)
                {
                    gameNameOk = true;
                }
                else
                {
                    Console.WriteLine("ingrese un nombre de juego valido");
                }
            }

            Console.WriteLine("califique con una valoracion del 0 al 5");
            bool starsOk = false;
            string stars = " ";
            while (!starsOk)
            {
                stars = Console.ReadLine();
                try
                {
                    int starsInt = Int32.Parse(stars);
                    if (starsInt >= 0 && starsInt <= 5) starsOk = true;
                    else Console.WriteLine("el numero de estrellas debe ser entre 0 y 5");
                }
                catch
                {
                     Console.WriteLine("debe ingresar un numero entre 0 y 5");
                }
            }
            Console.WriteLine("agregue un comentario");
            string comment = Console.ReadLine();
            string message = $"{gameName}#{stars}#{comment}";
            _communication.SendMessage(CommandConstants.PublishQualification, message);
            Console.WriteLine(_communication.ReceiveMessage().Message);
            MainMenu();
        }

        private void  ShowGameDetails() //agregar file receiver 
        {
        Console.WriteLine(_message.GameDetails);
        string gameName = Console.ReadLine();
        _communication.SendMessage(CommandConstants.GameDetails,gameName);
        Console.WriteLine(_communication.ReceiveMessage().Message);
        MainMenu();
        }
        private void ShowMyGames()
        {
            _communication.SendMessage(CommandConstants.MyGames,"");
            Console.WriteLine(_communication.ReceiveMessage().Message);
            Console.WriteLine(_message.MyGamesOptions);
        }

        private void SearchGame()
        {
            Console.WriteLine(_message.SearchGame);
            Console.WriteLine(_message.SearchByTitle);
            string title = " ";
            title = Console.ReadLine();
            Console.WriteLine(_message.SearchByGenre);
            string genre = " ";
            genre = Console.ReadLine();
            bool starsOk = false;
            string stars = " ";
            while (!starsOk)
            {
                Console.WriteLine(_message.SearchByStars);
                stars = Console.ReadLine();
                try
                {
                    int starsInt = Int32.Parse(stars);
                    if (starsInt >= 0 && starsInt <= 5) starsOk = true;
                    else Console.WriteLine("el numero de estrellas debe ser entre 0 y 5");
                }
                catch
                {
                    if (stars.Equals("")) starsOk = true;
                    else Console.WriteLine("debe ingresar un numero entre 0 y 5");
                }
            }

            string searchConcat = $"{title}#{genre}#{stars}";
            _communication.SendMessage(CommandConstants.SearchGame,searchConcat);
            Console.WriteLine(_message.SearchGameOptions);
            Console.WriteLine(_communication.ReceiveMessage().Message);
        }

        private void BuyGame()
        {
            Console.WriteLine(_message.BuyGame);
            string gameName = Console.ReadLine();
            _communication.SendMessage(CommandConstants.buyGame, gameName);
            Console.WriteLine(_communication.ReceiveMessage().Message);
            MainMenu();
        }

        private void MainMenu()
        {
            Console.WriteLine(_message.MainMenuMessage);
        }

        private void AddGame()
        {
            try
            {
                bool titleOk = false;
                string title = "";
                while (!titleOk)
                {
                    Console.WriteLine(_message.NewGameInit);
                    title = Console.ReadLine();
                    if (!string.IsNullOrEmpty(title)) titleOk = true;
                }

                bool genreOk = false;
                string genre = "";
                while (!genreOk)
                {
                    Console.WriteLine(_message.GameGenre);
                    genre = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(genre) || !string.IsNullOrWhiteSpace(genre)) genreOk = true;
                }
                
                bool synOk = false;
                string synopsis = "";
                while (!synOk)
                {
                    Console.WriteLine(_message.GameSynopsis);
                    synopsis =Console.ReadLine(); 
                    if (!string.IsNullOrWhiteSpace(synopsis) || !string.IsNullOrWhiteSpace(synopsis)) synOk = true;
                }
                bool ageOk = false;
                string ageRating = "";
                while (!ageOk)
                {
                    Console.WriteLine(_message.GameAgeRestriction);
                    ageRating =Console.ReadLine(); 
                    if (!string.IsNullOrWhiteSpace(synopsis) || !string.IsNullOrWhiteSpace(synopsis)) ageOk = true;
                }
                CommunicationSocket communication = new CommunicationSocket(_socket);
                string dataToSend = $"{title}#{genre}#{synopsis}#{ageRating}";
                communication.SendMessage(CommandConstants.AddGame,dataToSend);

                Console.WriteLine(""); //envio de imagen
                //aca falta enviar con file sender la imagen
                //                Console.WriteLine(_message.GameCover);
                //                string coverPath =Console.ReadLine();   
                Console.WriteLine(communication.ReceiveMessage().Message);
                Console.WriteLine(_message.MainMenuMessage);
            }
            catch (Exception e) //to be implemented
            {
                Console.WriteLine(e.Message);
            }
        }

        private void CloseConnection()
        {
            throw new NotImplementedException();
        }

        private void UserLogin()
        {
           Console.WriteLine(_message.UserLogIn);
           bool okLogin = false;
           bool menu = false;
           while (!okLogin&& !menu)
           {
               string user = Console.ReadLine();
               _communication.SendMessage(CommandConstants.LoginUser, user);
               CommunicatorPackage receive = _communication.ReceiveMessage();
               if (receive.Command != CommandConstants.userNotLogged) okLogin = true;
               Console.WriteLine(receive.Message);
           }
        }

        private void UserRegistration()
        {
            Console.WriteLine(_message.UserRegistration);
            string user = Console.ReadLine();
            if (user.Equals("") || user.Equals(" ")) Console.WriteLine("nombre no valido");
            else
            {
                _communication.SendMessage(CommandConstants.RegisterUser, user);
                Console.WriteLine(_communication.ReceiveMessage().Message);
            }
            StartUpMenu();
        }

        private void StartUpMenu()
        {
           Console.WriteLine(_message.StartUpMessage);
        }
    }
}
