using System;
using System.Net;
using System.Net.Sockets;
using Common.Communicator;
using Common.Protocol;
using Common.SettingsManager;
using Common.Communicator.Exceptions;
using Common.FileManagement.Exceptions;

namespace Client
{
    public class ClientManager
    {
        private Socket _socket;
        private IPEndPoint _remoteEndpoint;
        private CommunicationSocket _communication;
        private Message _message;
        private string _serverIpAddress;
        private string _serverPort;
        private bool _endConnection;

        public ClientManager()
        {
            _socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            _message = new SpanishMessage();

            ISettingsManager _ipConfiguration = new AddressIPConfiguration();
            _serverIpAddress = _ipConfiguration.ReadSetting("ServerIpAddress");
            _serverPort = _ipConfiguration.ReadSetting("ServerPort");

            _remoteEndpoint = new IPEndPoint(IPAddress.Parse(_serverIpAddress), int.Parse(_serverPort));

            this._communication = new CommunicationSocket(_socket);
        }
        
        public void Start()
        {
            try{
                _socket.Connect(_remoteEndpoint);
                Menu();
            }
            catch(SocketException){
                Console.WriteLine(_message.ServerClosed);
            }
        }
        
        private void Menu()
        {
            Console.WriteLine(_message.ClientConnectedWithServer);
            CommunicationSocket communication = new CommunicationSocket(_socket);
            _endConnection = false;
            try
            {
                communication.SendMessage(CommandConstants.StartupMenu, "");
                Console.WriteLine(communication.ReceiveMessage().Message);

                while (!_endConnection)
                {
                    string message = Console.ReadLine();
                    if (message.Equals("exit"))
                    {
                        _endConnection = true;
                    }
                    else
                    {
                        MessageInterpreter(message);
                    }
                }        
            }
            catch (SocketException)
            {
                Console.WriteLine(_message.ServerClosed);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            CloseConnection();
        }

        private void MessageInterpreter(string message)
        {
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
                        Console.WriteLine(_message.WrongOption);
                        break;
                }
            }
            catch(FormatException)
            {
                Console.WriteLine(_message.WrongOption);
            }
        }

        public void ShowCatalogue()
        {
            _communication.SendMessage(CommandConstants.ViewCatalogue, "");
            
            Console.WriteLine(_communication.ReceiveMessage().Message);
            Console.WriteLine(_message.SearchGameOptions);
        }

        private void PublishQualification()
        {
            Console.WriteLine(_message.PublishQualification);
            bool gameNameOk = false;
            string gameName = " ";
            while (!gameNameOk)
            {
                gameName = Console.ReadLine();
                _communication.SendMessage(CommandConstants.GameExists, gameName);
                int res = _communication.ReceiveMessage().Command;
                if (res == CommandConstants.GameExists)
                {
                    gameNameOk = true;
                }
                else
                {
                    Console.WriteLine(_message.InvalidTitle);
                }
            }

            Console.WriteLine(_message.SearchByStars);
            bool starsOk = false;
            string stars = " ";
            while (!starsOk)
            {
                stars = Console.ReadLine();
                try
                {
                    int starsInt = Int32.Parse(stars);
                    if (starsInt >= 0 && starsInt <= 5) starsOk = true;
                    else Console.WriteLine(_message.InvalidStars);
                }
                catch(FormatException)
                {
                    Console.WriteLine(_message.InvalidStars);
                }
            }

            Console.WriteLine(_message.QualificationComment); 
            string comment = Console.ReadLine();
            string message = $"{gameName}#{stars}#{comment}";
            _communication.SendMessage(CommandConstants.PublishQualification, message);
            Console.WriteLine(_communication.ReceiveMessage().Message);
            MainMenu();
        }

        private void  ShowGameDetails() 
        {
            Console.WriteLine(_message.GameDetails);
            string gameName = Console.ReadLine();
            
            _communication.SendMessage(CommandConstants.GameDetails, gameName);
            Console.WriteLine(_communication.ReceiveMessage().Message); 

            Console.WriteLine(_message.DownloadCover);

            string descargarCaratula = Console.ReadLine();
            int command; 
            if(descargarCaratula.Equals("1"))
            {
                command = CommandConstants.SendCover;
                _communication.SendMessage(command, gameName);
                string pathSavedAt = "";
                try
                {
                    pathSavedAt = _communication.ReceiveFile();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine(_message.SavedPathAt);
                Console.WriteLine(pathSavedAt);
            }

            MainMenu();
        }

        private void ShowMyGames()
        {
            _communication.SendMessage(CommandConstants.MyGames, "");

            CommunicatorPackage received = _communication.ReceiveMessage();
            if(received.Command !=  CommandConstants.userNotLogged)
            {
                string games = received.Message;
                ShowGameList(games);

                Console.WriteLine(_message.MyGamesOptions);
            }
            else{
                Console.WriteLine(received.Message);
            }
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
                    else Console.WriteLine(_message.InvalidStars);
                }
                catch(FormatException)
                {
                    if (stars.Equals("")) starsOk = true;
                    else Console.WriteLine(_message.InvalidStars);
                }
            }

            string searchConcat = $"{title}#{genre}#{stars}";
            _communication.SendMessage(CommandConstants.SearchGame, searchConcat);
            
            string games = _communication.ReceiveMessage().Message;
            ShowGameList(games);

            Console.WriteLine(_message.SearchGameOptions);
        }

        private void BuyGame()
        {
            Console.WriteLine(_message.BuyGame);
            string gameName = Console.ReadLine();
            _communication.SendMessage(CommandConstants.buyGame, gameName);
            Console.WriteLine(_communication.ReceiveMessage().Message);

            MainMenu();
        }

        private void AddGame()
        {
            bool titleOk = false;
            string title = "";
            while (!titleOk)
            {
                Console.WriteLine(_message.NewGameInit);
                title = Console.ReadLine();
                if (!string.IsNullOrEmpty(title))
                {
                    _communication.SendMessage(CommandConstants.GameExists, title);
                    int res = _communication.ReceiveMessage().Command;
                    if (res == CommandConstants.GameExists)
                    {
                        Console.WriteLine(_message.RepeatedGame);
                    }
                    else
                    {
                        titleOk = true;
                    }
                }
                else
                {
                    Console.WriteLine(_message.InvalidTitle);
                }
            }

            bool genreOk = false;
            string genre = "";
            while (!genreOk)
            {
                Console.WriteLine(_message.GameGenre);
                genre = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(genre))
                {
                    genreOk = true;
                } 
                else
                {
                    Console.WriteLine(_message.InvalidGenre);
                }
            }

            bool synOk = false;
            string synopsis = "";
            while (!synOk)
            {
                Console.WriteLine(_message.GameSynopsis);
                synopsis = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(synopsis)) 
                {
                    synOk = true;
                }
                else
                {
                    Console.WriteLine(_message.InvalidSynopsis);
                }
            }

            bool ageOk = false;
            string ageRating = "";
            while (!ageOk)
            {
                Console.WriteLine(_message.GameAgeRestriction);
                ageRating = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(synopsis))
                {
                    ageOk = true;
                } 
                else
                {
                    Console.WriteLine(_message.InvalidAge);
                }
            }

            string dataToSend = $"{title}#{genre}#{synopsis}#{ageRating}";
            _communication.SendMessage(CommandConstants.AddGame, dataToSend);

            CommunicatorPackage received = _communication.ReceiveMessage();
            Console.WriteLine(received.Message);

            if(received.Command !=  CommandConstants.userNotLogged){
                bool fileNotFound = true;
                while(fileNotFound)
                {
                    try{
                        string path = Console.ReadLine();
                        _communication.SendFile(path);
                        fileNotFound = false;
                    }
                    catch(FileNotFoundException){
                        Console.WriteLine(_message.FileNotFound);
                    }
                }
            
                Console.WriteLine(_communication.ReceiveMessage().Message);
                MainMenu();
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine(_message.Disconnected);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        private void UserLogin()
        {
            Console.WriteLine(_message.UserLogIn);
            bool okLogin = false;
            bool menu = false;
            while (!okLogin && !menu)
            {
                string user = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(user)) Console.WriteLine(_message.InvalidUsername);
                else
                {
                    _communication.SendMessage(CommandConstants.LoginUser, user);
                    CommunicatorPackage receive = _communication.ReceiveMessage();
                    if (receive.Command != CommandConstants.userNotLogged) okLogin = true;
                    Console.WriteLine(receive.Message);
                }
            }
            MainMenu();
        }

        private void UserRegistration()
        {
            Console.WriteLine(_message.UserRegistration);
            string user = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(user)) Console.WriteLine(_message.InvalidUsername);
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

        private void MainMenu()
        {
            Console.WriteLine("\n" + _message.MainMenuMessage);
        }

        private void ShowGameList(string games)
        {
            if(string.IsNullOrEmpty(games))
            {
                Console.WriteLine(_message.GamesNotFound);
            }
            else
            {
                Console.WriteLine(_message.GameList);
                Console.WriteLine(games);
            }
        }
    }
}