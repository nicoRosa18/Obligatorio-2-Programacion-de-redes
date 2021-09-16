using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using Common.Protocol;
using Server.Domain.ServerExceptions;

namespace Server.Domain
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

        public void MessageInterpreter(Header header)
        {
            string messageReturn = "";
            switch (header.ICommand)
            {
                case CommandConstants.StartupMenu:
                    StartUpMenu();
                    break;
                case CommandConstants.RegisterUser:
                    UserRegistration(header.IDataLength);
                    break;
                case CommandConstants.LoginUser:
                    UserLogin(header.IDataLength);
                    break;
                // case CommandConstants.ViewCatalogue:
                //     ShowCatalogue();
                //     break;
                // case CommandConstants.MainMenu:
                //     mainMenu();
                //     break;
                // case CommandConstants.AddGame:
                //     AddGame();
                //     break;
                // case "":
                //     CloseConnection();
                //     break;
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
        
        private void UserRegistration(int datalength)
        {
            SendMessage(_message.UserRegistration);
            string userName = ReceiveMessage(datalength);

            if (_usersAndCatalogueManager.ContainsUser(userName))
                SendMessage(_message.UserRepeated);
                
            else
            {
                _usersAndCatalogueManager.AddUser(userName);
                SendMessage(_message.UserCreated + _usersAndCatalogueManager.Users.Count + "\n" + _message.BackToStartUpMenu);
            }            
        }

        private void UserLogin(int dataLength)
        {
            SendMessage(_message.UserLogIn);
            string user = ReceiveMessage(dataLength);
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
                    // SendMessage(_message.NewGameInit);
                    // string title = ReceiveMessage();
                    // SendMessage(_message.GameGenre);
                    // string genre = ReceiveMessage();
                    // SendMessage(_message.GameSynopsis);
                    // string synopsis = ReceiveMessage();      
                    // SendMessage(_message.GameAgeRestriction);
                    // string ageRating = ReceiveMessage();
                    // SendMessage(_message.GameCover);
                    // string coverPath = ReceiveMessage();

                    // Game gameToAdd = new Game(title, coverPath, genre, synopsis, ageRating);
                    // _usersAndCatalogueManager.AddGame(gameToAdd);

                    // SendMessage(_message.GameAdded);
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

        private string ReceiveMessage(int dataLength)
        {
            var bufferData = new byte[dataLength];  
            ReceiveData(this.ConnectedSocket, dataLength, bufferData);
            
            return Encoding.UTF8.GetString(bufferData);
        }

        

        private void SendMessage(string message)
        {
            var header = new Header(HeaderConstants.Request, CommandConstants.Message, message.Length);
            var data = header.GetRequest();

            var sentBytes = 0;
            while (sentBytes < data.Length)
            {
                sentBytes += this.ConnectedSocket.Send(data, sentBytes, data.Length - sentBytes, SocketFlags.None);
            }

            sentBytes = 0;
            var bytesMessage = Encoding.UTF8.GetBytes(message);
            while (sentBytes < bytesMessage.Length)
            {
                sentBytes += this.ConnectedSocket.Send(bytesMessage, sentBytes, bytesMessage.Length - sentBytes,
                    SocketFlags.None);
            }
        }

        private void CloseConnection()
        {
            this.Active = false;
        }

        public void Listen(Socket clientSocket)
        {  
            int headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                   HeaderConstants.DataLength;
            var buffer = new byte[headerLength];
            try
            {
                ReceiveData(clientSocket, headerLength, buffer);
                Header header = new Header();
                header.DecodeData(buffer);
                this.MessageInterpreter(header);
            }
            catch (ServerClosingException e)
            {
                CloseConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}..");    
            }
        }

        private void ReceiveData(Socket clientSocket,  int length, byte[] buffer)
        {
            var iRecv = 0;
            while (iRecv < length)
            {
                try
                {
                    var localRecv = clientSocket.Receive(buffer, iRecv, length - iRecv, SocketFlags.None);
                    if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                    {
                        throw new ServerClosingException();
                    }

                    iRecv += localRecv;
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se.Message);
                    return;
                }
            }
        }
    }
}