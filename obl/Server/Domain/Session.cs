using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using Common.Protocol;

namespace ConsoleAppSocketServer.Domain
{
    public class Session
    {
        public int ThreadId { get; set; }

        public bool Active { get; set; }

        public static UsersAndCatalogueManager _usersAndCatalogueManager { get; set; }

        private Socket ConnectedSocket { get; set; }

        private User _userLogged { get; set; }

        private Message _message { get; set; }

        public Session(Socket connectedSocket, int threadId, Message message)
        {
            this.ThreadId = threadId;
            this.ConnectedSocket = connectedSocket;
            this.Active = true;
            this._message = message;
        }

        public void MessageInterpreter(string message)
        {
            string messageReturn = "";
            switch (message)
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
                case CommandConstants.ViewCatalogue:
                    ShowCatalogue();
                    break;
                case CommandConstants.MainMenu:
                    mainMenu();
                    break;
                case CommandConstants.AddGame:
                    AddGame();
                    break;
                case "":
                    CloseConnection();
                    break;
                default:
                    messageReturn = "por favor envie una opcion correcta";
                    SendMessage(messageReturn);
                    break;
            }
        }

        private void StartUpMenu()
        {
            string messageToSend = _message.StartUpMessage;
            SendMessage(messageToSend);
        }
        
        private void UserRegistration()
        {
            SendMessage(_message.UserRegistration);
            string userName = Receive();

            if (_usersAndCatalogueManager.ContainsUser(userName))
                SendMessage(_message.UserRepeated);
                
            else
            {
                _usersAndCatalogueManager.AddUser(userName);
                SendMessage(_message.UserCreated + _usersAndCatalogueManager.Users.Count + "\n" + _message.BackToStartUpMenu);
            }            
        }

        private void UserLogin()
        {
            SendMessage(_message.UserLogIn);
            string user = Receive();
            if (_usersAndCatalogueManager.Login(user))
            {
                _userLogged = _usersAndCatalogueManager.GetUser(user);
                mainMenu();
            }
            else
            {
                SendMessage(_message.UserIncorrect);
            }
        }

        private void ShowCatalogue()
        {
            if (_userLogged != null)
            {
                Catalogue catalogue = _usersAndCatalogueManager.GetCatalogue();

                string messageToSend = catalogue.ShowGamesOnStringList();
                if(messageToSend.Equals("")){
                    messageToSend = _message.EmptyCatalogue;
                }
                SendMessage(_message.CatalogueView + messageToSend);
            }
            else
            {
                SendMessage((_message.InvalidOption));
            }
        }

        private void mainMenu()
        {
            if (_userLogged != null)
            {
                string messageToSend = _message.MainMenuMessage;
                SendMessage(messageToSend);
            }
            else
            {
                SendMessage((_message.InvalidOption));
            }
        }

        private void AddGame()
        {
            if (_userLogged != null)
            {
                try
                {
                    SendMessage(_message.NewGameInit);
                    string title = Receive();
                    SendMessage(_message.GameGenre);
                    string genre = Receive();
                    SendMessage(_message.GameSynopsis);
                    string synopsis = Receive();      
                    SendMessage(_message.GameAgeRestriction);
                    string ageRating = Receive();
                    SendMessage(_message.GameCover);
                    string coverPath = Receive();

                    Game gameToAdd = new Game(title, coverPath, genre, synopsis, ageRating);
                    _usersAndCatalogueManager.AddGame(gameToAdd);

                    SendMessage(_message.GameAdded);
                }
                catch (Exception e) //to be implemented
                {
                    SendMessage(e.Message);
                }
            }
            else
            {
                SendMessage((_message.InvalidOption));
            }
        }

        private string Receive()
        {
            var buffer = new byte[1024];
            var bytesReceived = 1;
            bytesReceived = this.ConnectedSocket.Receive(buffer);
            if (bytesReceived > 0)
            {
                var message = Encoding.UTF8.GetString(buffer);
                return message;
            }

            throw new Exception("empty message");
        }

        

        private void SendMessage(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message+"*");
            this.ConnectedSocket.Send(messageBytes);
        }

        private void CloseConnection()
        {
            this.Active = false;
        }

        public void Listen()
        {
            Console.WriteLine("en Listen de Session");
            // Este while se usa para mantenerse aqui mientras la conexion no se cierra
            var buffer = new byte[1024];
                // Si la conexion se cierra, el receive retorna 0
             var   bytesReceived = ConnectedSocket.Receive(buffer);
                if (bytesReceived > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer);
                    var messagecleared = MessageManager.EliminarEspacios(message);
                    Console.WriteLine(messagecleared);
                    this.MessageInterpreter(messagecleared); //interpreta el mensaje recibido y genera una respuesta
                }
                else
                {
                    Console.WriteLine($"{this.ThreadId}: El cliente remoto cerro la conexion...");
                }
        }
    }
}