using System;
using Common.Protocol;
using Server.Domain.ServerExceptions;
using Common.Communicator.Exceptions;
using Common.Communicator;
using Common.FileManagement.Exceptions;
using Server.Domain;

namespace Server
{
    public class Session
    {
        public bool Active { get; set; }

        public static UsersAndCatalogueManager _usersAndCatalogueManager { get; set; }

        private User _userLogged;

        private ICommunicator _communicator;

        private Message _messageLanguage;

        public Session(ICommunicator communicator, Message messageLanguage)
        {
            this.Active = true;
            this._communicator = communicator;
            this._messageLanguage = messageLanguage;
        }

        public void LogOut()
        {
            if(_userLogged != null)
            {
                _userLogged.LogOut();
            }
        }

        private void MessageInterpreter(CommunicatorPackage package)
        {
            string messageReturn = "";
            switch (package.Command)
            {
                case CommandConstants.StartupMenu:
                    StartUpMenu();
                    break;
                case CommandConstants.RegisterUser:
                    UserRegistration(package);
                    break;
                case CommandConstants.LoginUser:
                    UserLogin(package);
                    break;
                case CommandConstants.ViewCatalogue:
                    ShowCatalogue();
                    break;
                case CommandConstants.AddGame:
                    AddGame(package);
                    break;
                case CommandConstants.SearchGame:
                    SearchGame(package);
                    break;
                case CommandConstants.buyGame:
                    BuyGame(package);
                    break;
                case CommandConstants.RemoveGame:
                    RemoveGame(package);
                    break;
                case CommandConstants.ModifyGame:
                    ModifyGame(package);
                    break;
                case CommandConstants.MyGames:
                    MyGames();
                    break;
                case CommandConstants.GameDetails:
                    ViewGameDetails(package);
                    break;
                case  CommandConstants.PublishQualification:
                    PublishQualification(package);
                    break;
                case CommandConstants.GameExists:
                    GameExists(package);
                    break;
                case CommandConstants.SendCover:
                    SendCover(package);
                    break;
                default:
                    messageReturn = "por favor envie una opcion correcta";
                    _communicator.SendMessage(CommandConstants.StartupMenu, messageReturn);
                    break;
            }
        }

        private void GameExists(CommunicatorPackage package)
        {
            try
            {
                Game gameBeacon = new Game();
                gameBeacon.Title = package.Message;
                _usersAndCatalogueManager.ExistsGame(gameBeacon);
                _communicator.SendMessage(CommandConstants.GameNotExits, _messageLanguage.GameNotFound);
            }
            catch (GameAlreadyExists)
            {
                _communicator.SendMessage(CommandConstants.GameExists,"");
            }
        }

        private void RemoveGame(CommunicatorPackage package)
        {
            if(_userLogged != null)
            {
                try
                {
                    Game removeGameBeacon = new Game();
                    removeGameBeacon.Title = package.Message;
                    _usersAndCatalogueManager.RemoveGame(_userLogged, removeGameBeacon);
                    _communicator.SendMessage(CommandConstants.RemoveGame, _messageLanguage.GameRemovedCorrectly);
                }
                catch(UserNotOwnerofGame)
                {
                    _communicator.SendMessage(CommandConstants.RemoveGame, _messageLanguage.UserNotGameOwner);
                }
            }
            else
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void ModifyGame(CommunicatorPackage package)
        {
            if(_userLogged != null)
            {
                string[] data= new string[5];
                data = package.Message.Split("#");
                string oldTitle = data[0];
                string newTitle = data[1];
                string newGenre = data[2];
                string newSynopsis = data[3];     
                string newAgeRating = data[4];

                string newCover = "";

                _communicator.SendMessage(CommandConstants.SendCover, _messageLanguage.SendGameCover);
                if(_communicator.ReceiveMessage().Command == CommandConstants.ReceiveCover)
                {
                    newCover = _communicator.ReceiveFile();
                }
                Game newGame = new Game(newTitle, newCover, newGenre, newSynopsis, newAgeRating);
                Game oldGameBeacon = new Game();
                oldGameBeacon.Title = oldTitle;

                try
                {
                    _usersAndCatalogueManager.ModifyGame(_userLogged, oldGameBeacon, newGame);
                    _communicator.SendMessage(CommandConstants.ModifyGame, _messageLanguage.GameModifiedCorrectly);
                }
                catch(UserNotOwnerofGame)
                {
                    _communicator.SendMessage(CommandConstants.ModifyGame, _messageLanguage.UserNotGameOwner);
                }
            }
            else
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void PublishQualification(CommunicatorPackage package)
        {
            if(_userLogged != null)
            {
                string[] data = new string[3];
                data = package.Message.Split("#");
                string gameName = data[0];
                int stars= Int32.Parse(data[1]);
                string comment = data[2];

                Qualification q = new Qualification();
                q.Comment = comment;
                q.Stars = stars;
                q.User = _userLogged.Name;
                _usersAndCatalogueManager.AddCommunityQualification(gameName, q);
                _communicator.SendMessage(CommandConstants.PublishQualification, _messageLanguage.QualificationAdded);
            }
            else
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void ViewGameDetails(CommunicatorPackage package)
        {
            try
            {
                Game game = _usersAndCatalogueManager.GetGame(package.Message);
                GameDetails gameDetails = new GameDetails(game);
                _communicator.SendMessage(CommandConstants.GameDetails, gameDetails.DetailsOnstring());
            }
            catch (GameNotFound)
            {
                _communicator.SendMessage(package.Command, _messageLanguage.GameNotFound);
            }
        }

        private void SendCover(CommunicatorPackage package)
        {    
            Game game = _usersAndCatalogueManager.GetGame(package.Message);
            string path = game.Cover;

            try{
                _communicator.SendFile(path);
            }
            catch(FileNotFoundException)
            {
                _communicator.SendFile(path); //hacer una imagen comun que sea un ejemplo o hablar q hacer
            }
        }

        private void MyGames()
        {
            if(_userLogged != null)
            {
                _communicator.SendMessage(CommandConstants.MyGames,_userLogged.GetMyGames());
            }
            else
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void SearchGame(CommunicatorPackage package)
        {
            string[] values = new string[3];
            values = package.Message.Split("#");
            string title = values[0];
            string genre = values[1];
            int stars = -1;
            try
            {
                stars = Int32.Parse(values[2]);
            }
            catch { }

            try
            {
                string games = _usersAndCatalogueManager.SearchGames(title, genre, stars);
                _communicator.SendMessage(CommandConstants.SearchGame, games);
            }
            catch (GameNotFound)
            {
                _communicator.SendMessage(CommandConstants.GameNotExits, _messageLanguage.GameNotFound);
            }
        }

        private void StartUpMenu()
        {
            string messageToSend = _messageLanguage.StartUpMessage;
            _communicator.SendMessage(CommandConstants.StartupMenu, messageToSend);
        }
        
        private void UserRegistration(CommunicatorPackage received)
        {
            string userName = received.Message;
            try
            {
                _usersAndCatalogueManager.ContainsUser(userName);
                _communicator.SendMessage(CommandConstants.RegisterUser, _messageLanguage.UserRepeated);
            }
            catch(UserNotFound)
            {
                _usersAndCatalogueManager.AddUser(userName);
                _communicator.SendMessage(CommandConstants.RegisterUser, _messageLanguage.UserCreated + _usersAndCatalogueManager.Users.Count + "\n");
            }            
        }

        private void UserLogin(CommunicatorPackage received)
        {
            string user = received.Message;
            try
            {
                _userLogged = _usersAndCatalogueManager.Login(user);
                _communicator.SendMessage(CommandConstants.userLogged, _messageLanguage.UserLogged);
            }
            catch(UserNotFound)
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserIncorrect);
            }
            catch(UserAlreadyLoggedIn)
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserAlreadyLoggedIn);
            }
        }

        private void BuyGame(CommunicatorPackage package)
        {
            if(_userLogged != null)
            {
                try
                {
                    Game game = _usersAndCatalogueManager.GetGame(package.Message);
                    _userLogged.BuyGame(game.Title);
                    _communicator.SendMessage(CommandConstants.buyGame, _messageLanguage.GamePurchased);
                }
                catch(GameNotFound)
                {
                    _communicator.SendMessage(CommandConstants.buyGame, _messageLanguage.GameNotFound);
                }
                catch(GameAlreadyPurchased)
                {
                    _communicator.SendMessage(CommandConstants.buyGame, _messageLanguage.GameAlreadyInLibrary);
                }
            }
            else
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void ShowCatalogue()
        {
            string messageToSend = _usersAndCatalogueManager.GetCatalogue();

            if(messageToSend.Equals("")){
                messageToSend = _messageLanguage.EmptyCatalogue;
            }
            _communicator.SendMessage(CommandConstants.ViewCatalogue, _messageLanguage.CatalogueView + messageToSend);
        }

        private void AddGame(CommunicatorPackage package)
        {
            if (_userLogged != null)
            {
                string[] data= new string[4];
                data = package.Message.Split("#");
                string title = data[0];
                string genre = data[1];
                string synopsis =data[2];     
                string ageRating = data[3];
                _communicator.SendMessage(CommandConstants.SendCover, _messageLanguage.SendGameCover);
                bool okReceived = false;
                string coverPath = "Default";
                while (!okReceived)
                {
                    try
                    {
                        coverPath = _communicator.ReceiveFile();
                        okReceived = true;
                    }
                    catch (Exception e)
                    {
                        _communicator.SendMessage(CommandConstants.SendCover,_messageLanguage.ErrorGameCover);
                    }
                }

                Game gameToAdd = new Game(title, coverPath, genre, synopsis, ageRating);

                _usersAndCatalogueManager.AddGame(_userLogged, gameToAdd);

                _communicator.SendMessage(CommandConstants.AddGame, _messageLanguage.GameAdded);
            }
            else
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        public void Listen()
        {  
            try
            {
                this.MessageInterpreter(this._communicator.ReceiveMessage());
            }
            catch (ClientClosingException)
            {
                CloseSession();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message}.."); 
                CloseSession();
            }
        }

        private void CloseSession()
        {
            this.Active = false;
            this.LogOut();
        }
               
    }
}