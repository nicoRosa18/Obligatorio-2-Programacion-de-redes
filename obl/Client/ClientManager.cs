using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Common.Communicator;
using Common.Protocol;
using Common.SettingsManager;
using Common.Communicator.Exceptions;
using System.Threading.Tasks;

namespace Client
{
    public class ClientManager
    {
        private TcpClient _tcpClient;
        private IPEndPoint _remoteEndpoint;
        private IPEndPoint _localEndpoint;
        private CommunicationTcp _communication;
        private Message _message;
        private string _clientIpAddress;
        private string _clientPort;
        private string _serverIpAddress;
        private string _serverPort;
        private bool _endConnection;
        private ISettingsManager _pathsManager;

        public ClientManager()
        {
            _message = new SpanishMessage();

            ISettingsManager _ipConfiguration = new AddressIPConfiguration();
            _clientIpAddress = _ipConfiguration.ReadSetting("ClientIpAddress");
            _clientPort = _ipConfiguration.ReadSetting("ClientPort");
            _serverIpAddress = _ipConfiguration.ReadSetting("ServerIpAddress");
            _serverPort = _ipConfiguration.ReadSetting("ServerPort");

            _localEndpoint = new IPEndPoint(IPAddress.Parse(_clientIpAddress), int.Parse(_clientPort));

            _tcpClient = new TcpClient(_localEndpoint);

            _pathsManager = new PathsConfiguration();

            System.IO.Directory.CreateDirectory(_pathsManager.ReadSetting("CoversPath"));
        }

        public async Task StartAsync()
        {
            try
            {
                await _tcpClient.ConnectAsync(IPAddress.Parse(_serverIpAddress), int.Parse(_serverPort))
                    .ConfigureAwait(false);
                ;
                this._communication = new CommunicationTcp(_tcpClient);
                Menu();
            }
            catch (SocketException)
            {
                Console.WriteLine(_message.ServerClosed);
            }
        }

        private void Menu()
        {
            Console.WriteLine(_message.ClientConnectedWithServer);
            CommunicationTcp communication = new CommunicationTcp(_tcpClient);
            _endConnection = false;
            try
            {
                communication.SendMessageAsync(CommandConstants.StartupMenu, "");

                CommunicatorPackage StartupMenu = communication.ReceiveMessageAsync().Result;
                Console.WriteLine(StartupMenu.Message);

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
            catch (AggregateException)
            {
                _tcpClient.Close();
                Console.WriteLine(_message.ServerClosed);
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
                    case CommandConstants.RemoveGame:
                        RemoveGame();
                        break;
                    case CommandConstants.ModifyGame:
                        ModifyGame();
                        break;
                    case CommandConstants.PublishQualification:
                        PublishQualification();
                        break;
                    case CommandConstants.ViewCatalogue:
                        ShowCatalogue();
                        break;
                    default:
                        Console.WriteLine(_message.WrongOption);
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine(_message.WrongOption);
            }
        }

        private void ShowCatalogue()
        {
            _communication.SendMessageAsync(CommandConstants.ViewCatalogue, "");

            CommunicatorPackage showCatalogue = _communication.ReceiveMessageAsync().Result;
            Console.WriteLine(showCatalogue.Message);
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
                _communication.SendMessageAsync(CommandConstants.GameExists, gameName);

                CommunicatorPackage gameExists = _communication.ReceiveMessageAsync().Result;
                if (gameExists.Command == CommandConstants.GameExists)
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
                catch (FormatException)
                {
                    Console.WriteLine(_message.InvalidStars);
                }
            }

            Console.WriteLine(_message.QualificationComment);
            string comment = Console.ReadLine();
            string message = $"{gameName}#{stars}#{comment}";
            _communication.SendMessageAsync(CommandConstants.PublishQualification, message);

            CommunicatorPackage publishQualification = _communication.ReceiveMessageAsync().Result;
            Console.WriteLine(publishQualification.Message);
            MainMenu();
        }

        private void ShowGameDetails()
        {
            Console.WriteLine(_message.GameDetails);
            string gameName = Console.ReadLine();
            _communication.SendMessageAsync(CommandConstants.GameDetails, gameName);
            CommunicatorPackage gameDetails = _communication.ReceiveMessageAsync().Result;
            if (gameDetails.Command == CommandConstants.GameNotExits)
            {
                Console.WriteLine(gameDetails.Message);
            }
            else
            {
                Console.WriteLine(_message.DownloadCover);

                string descargarCaratula = Console.ReadLine();
                int command;
                if (descargarCaratula.Equals("1"))
                {
                    _communication.SendMessageAsync(CommandConstants.GameExists, "");
                    CommunicatorPackage gameExists = _communication.ReceiveMessageAsync().Result;
                    if (gameExists.Command == CommandConstants.GameExists)
                    {
                        command = CommandConstants.SendCover;
                        _communication.SendMessageAsync(command, gameName);
                        string pathSavedAt = "";
                        try
                        {
                            string path = _pathsManager.ReadSetting("CoversPath");
                            pathSavedAt = _communication.ReceiveFileAsync(path).Result;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        Console.WriteLine(_message.SavedPathAt);
                        Console.WriteLine(pathSavedAt);
                    }
                    else
                    {
                        Console.WriteLine(_message.GameNotFound);
                    }
                }
            }

            MainMenu();
        }

        private void ShowMyGames()
        {
            _communication.SendMessageAsync(CommandConstants.MyGames, "");

            CommunicatorPackage myGamesReceived = _communication.ReceiveMessageAsync().Result;
            if (myGamesReceived.Command != CommandConstants.userNotLogged)
            {
                string games = myGamesReceived.Message;
                ShowGameList(games);

                Console.WriteLine(_message.MyGamesOptions);
            }
            else
            {
                Console.WriteLine(myGamesReceived.Message);
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
                catch (FormatException)
                {
                    if (stars.Equals("")) starsOk = true;
                    else Console.WriteLine(_message.InvalidStars);
                }
            }

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(stars))
            {
                Console.WriteLine(_message.NoParametersToSearch);
                MainMenu();
            }
            else
            {
                string searchConcat = $"{title}#{genre}#{stars}";
                _communication.SendMessageAsync(CommandConstants.SearchGame, searchConcat);

                CommunicatorPackage searchGame = _communication.ReceiveMessageAsync().Result;
                string games = searchGame.Message;
                ShowGameList(games);
                Console.WriteLine(_message.ChangeMenu);
            }
        }

        private void BuyGame()
        {
            Console.WriteLine(_message.BuyGame);
            string gameName = Console.ReadLine();
            _communication.SendMessageAsync(CommandConstants.buyGame, gameName);

            CommunicatorPackage buyGame = _communication.ReceiveMessageAsync().Result;
            Console.WriteLine(buyGame.Message);

            MainMenu();
        }

        private void RemoveGame()
        {
            Console.WriteLine(_message.GameDeleted);

            string gameToDelete = Console.ReadLine();
            _communication.SendMessageAsync(CommandConstants.GameExists, gameToDelete);

            CommunicatorPackage existsPackage = _communication.ReceiveMessageAsync().Result;
            if (existsPackage.Command == CommandConstants.GameExists)
            {
                _communication.SendMessageAsync(CommandConstants.RemoveGame, gameToDelete);

                CommunicatorPackage removeGame = _communication.ReceiveMessageAsync().Result;
                Console.WriteLine(removeGame.Message);
            }
            else
            {
                Console.WriteLine(existsPackage.Message);
            }

            MainMenu();
        }

        private void ModifyGame()
        {
            Console.WriteLine(_message.GameModified);
            string oldGameTitle = Console.ReadLine();
            _communication.SendMessageAsync(CommandConstants.GameExists, oldGameTitle);
            CommunicatorPackage existsPackage = _communication.ReceiveMessageAsync().Result;

            if (existsPackage.Command == CommandConstants.GameExists)
            {
                ModifyGameData(oldGameTitle);
            }
            else
            {
                Console.WriteLine(existsPackage.Message);
            }

            MainMenu();
        }

        private void ModifyGameData(string oldGameTitle)
        {
            Console.WriteLine(_message.GameModifiedNewInfo);

            bool titleOk = false;
            string title = "";
            while (!titleOk)
            {
                Console.WriteLine(_message.NewGameInit);
                title = Console.ReadLine();

                _communication.SendMessageAsync(CommandConstants.GameExists, title);

                CommunicatorPackage gameExists = _communication.ReceiveMessageAsync().Result;
                int res = gameExists.Command;
                if (res == CommandConstants.GameExists)
                {
                    Console.WriteLine(_message.RepeatedGame);
                }
                else
                {
                    titleOk = true;
                }
            }

            string genre = "";
            Console.WriteLine(_message.GameGenre);
            genre = Console.ReadLine();

            Console.WriteLine(_message.GameSynopsis);
            string synopsis = "";
            synopsis = Console.ReadLine();
            string ageRating = "";
            try
            {
                Console.WriteLine(_message.GameAgeRestriction);
                ageRating = Console.ReadLine();
                if (!string.IsNullOrEmpty(ageRating))
                    Int32.Parse(ageRating);
            }
            catch (FormatException)
            {
                Console.WriteLine(_message.InvalidAge);
            }

            string dataToSend = $"{oldGameTitle}#{title}#{genre}#{synopsis}#{ageRating}";

            _communication.SendMessageAsync(CommandConstants.ModifyGame, dataToSend);

            CommunicatorPackage received = _communication.ReceiveMessageAsync().Result;
            Console.WriteLine(received.Message);

            if (received.Command != CommandConstants.userNotLogged)
            {
                string option = Console.ReadLine();
                if (option.Equals("1"))
                {
                    _communication.SendMessageAsync(CommandConstants.ReceiveCover, "");
                    bool fileNotFound = true;
                    string path;
                    while (fileNotFound)
                    {
                        try
                        {
                            path = Console.ReadLine();
                            _communication.SendFileAsync(path).Wait();
                            fileNotFound = false;
                        }
                        catch (AggregateException e)
                        {
                            if (e.InnerExceptions[0] is FileNotFoundException)
                                Console.WriteLine(_message.FileNotFound);
                        }
                    }
                }
                else
                {
                    _communication.SendMessageAsync(CommandConstants.NoModifyCover, "");
                }

                CommunicatorPackage noModifyCover = _communication.ReceiveMessageAsync().Result;
                Console.WriteLine(noModifyCover.Message);
            }
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
                    _communication.SendMessageAsync(CommandConstants.GameExists, title);

                    CommunicatorPackage gameExists = _communication.ReceiveMessageAsync().Result;
                    if (gameExists.Command == CommandConstants.GameExists)
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
                try
                {
                    if (!string.IsNullOrWhiteSpace(ageRating))
                    {
                        Int32.Parse(ageRating);
                        ageOk = true;
                    }
                    else
                    {
                        Console.WriteLine(_message.InvalidAge);
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine(_message.InvalidAge);
                }
            }

            string dataToSend = $"{title}#{genre}#{synopsis}#{ageRating}";
            _communication.SendMessageAsync(CommandConstants.AddGame, dataToSend);

            CommunicatorPackage received = _communication.ReceiveMessageAsync().Result;
            Console.WriteLine(received.Message);
            if (received.Command != CommandConstants.userNotLogged)
            {
                bool fileNotFound = true;
                while (fileNotFound)
                {
                    try
                    {
                        string path = Console.ReadLine();
                        _communication.SendFileAsync(path).Wait();
                        fileNotFound = false;
                    }
                    catch (AggregateException e)
                    {
                        if (e.InnerExceptions[0] is FileNotFoundException)
                            Console.WriteLine(_message.FileNotFound);
                    }
                }

                CommunicatorPackage addGame = _communication.ReceiveMessageAsync().Result;
                Console.WriteLine(addGame.Message);
                MainMenu();
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine(_message.Disconnected);
            _tcpClient.Close();
        }

        private void UserLogin()
        {
            Console.WriteLine(_message.UserLogIn);
            bool okLogin = false;
            while (!okLogin)
            {
                string user = Console.ReadLine();
                if (user.Equals(CommandConstants.StartupMenu.ToString())) break;
                if (string.IsNullOrWhiteSpace(user)) Console.WriteLine(_message.InvalidUsername);
                else
                {
                    _communication.SendMessageAsync(CommandConstants.LoginUser, user);

                    CommunicatorPackage receive = _communication.ReceiveMessageAsync().Result;
                    if (receive.Command != CommandConstants.userNotLogged) okLogin = true;
                    Console.WriteLine(receive.Message);
                }
            }

            if (okLogin)
                MainMenu();
            else
                StartUpMenu();
        }

        private void UserRegistration()
        {
            Console.WriteLine(_message.UserRegistration);
            string user = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(user)) Console.WriteLine(_message.InvalidUsername);
            else
            {
                _communication.SendMessageAsync(CommandConstants.RegisterUser, user);

                CommunicatorPackage registerUser = _communication.ReceiveMessageAsync().Result;
                Console.WriteLine(registerUser.Message);
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
            if (string.IsNullOrEmpty(games))
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