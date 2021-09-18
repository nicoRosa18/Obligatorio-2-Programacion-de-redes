using System;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Common.Communicator;
using Common.Protocol;

namespace Client
{
    public class ClientManager
    {

        private Socket _socket { get; set; }
        private IPEndPoint _remoteEndpoint { get; set; }
        
        private CommunicationSocket _communication { get; set; }

        private Message _message { get; set; }

        public ClientManager(Socket socketClient)
        {
            this._message = new SpanishMessage();
            this._socket = socketClient;
            _remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.5"), 30000);
            this._communication= new CommunicationSocket(_socket);   
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
            string gameName = Console.ReadLine();
            //aca verificar que el nombre del juego exista.
            Console.WriteLine("califique con una valoracion del 0 al 5");
            bool starsOk = false;
            string stars = "";
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
            _communication.SendMessage(CommandConstants.PublishQualification, comment);
            Console.WriteLine(_communication.ReceiveMessage().Message);
            MainMenu();
        }

        private void  ShowGameDetails()
        {
        Console.WriteLine(_message.GameDetails);
        string gameName = Console.ReadLine();
        _communication.SendMessage(CommandConstants.GameDetails,gameName);
        _communication.ReceiveMessage();
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
            string option = Console.ReadLine();
            int optionInt = Int32.Parse(option);
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
                Console.WriteLine(_message.NewGameInit);
                string title = Console.ReadLine();
                Console.WriteLine(_message.GameGenre);
                string genre = Console.ReadLine();    
                Console.WriteLine(_message.GameSynopsis);
                string synopsis =Console.ReadLine();       
                Console.WriteLine(_message.GameAgeRestriction);
                string ageRating =Console.ReadLine();    
                Console.WriteLine(_message.GameCover);
                string coverPath =Console.ReadLine();    
                CommunicationSocket communication = new CommunicationSocket(_socket);
                string dataToSend = $"{title}#{genre}#{synopsis}#{ageRating}";
                communication.SendMessage(CommandConstants.AddGame,dataToSend);
                //aca falta enviar con file sender la imagen
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
           string user = Console.ReadLine();
           _communication.SendMessage(CommandConstants.LoginUser,user);
           Console.WriteLine(_communication.ReceiveMessage().Message);
        }

        private void UserRegistration()
        {
            Console.WriteLine(_message.UserRegistration);
            string user = Console.ReadLine();
            _communication.SendMessage(CommandConstants.RegisterUser, user); 
            Console.WriteLine(_communication.ReceiveMessage().Message);
            StartUpMenu();
        }

        private void StartUpMenu()
        {
           Console.WriteLine(_message.StartUpMessage);
        }
    }
    }
