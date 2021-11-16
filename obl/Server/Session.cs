using System;
using System.Drawing;
using Common.Protocol;
using Server.Domain.ServerExceptions;
using Common.Communicator.Exceptions;
using Common.Communicator;
using Common.FileManagement;
using Common.SettingsManager;
using Server.Domain;
using System.Threading.Tasks;
using Server.MessageQueue;
using CommonLogs;

namespace Server
{
    public class Session
    {
        public bool Active { get; set; }

        public static UsersAndCatalogueManager _usersAndCatalogueManager { get; set; }

        public static LocalSender _localSender { get; set; }

        private User _userLogged;

        private ICommunicator _communicator;

        private Message _messageLanguage;

        private ISettingsManager _pathsManager;
        

        public Session(ICommunicator communicator, Message messageLanguage)
        {
            this.Active = true;
            this._communicator = communicator;
            this._messageLanguage = messageLanguage;
            this._pathsManager = new PathsConfiguration();
        }

        public void LogOut()
        {
            if (_userLogged != null)
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
                case CommandConstants.PublishQualification:
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
                    _communicator.SendMessageAsync(CommandConstants.StartupMenu, messageReturn);
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
                _communicator.SendMessageAsync(CommandConstants.GameNotExits, _messageLanguage.GameNotFound);
            }
            catch (GameAlreadyExists)
            {
                _communicator.SendMessageAsync(CommandConstants.GameExists, "");
            }
        }

        private void RemoveGame(CommunicatorPackage package)
        {
            if (_userLogged != null)
            {
                try
                {
                    Game removeGameBeacon = new Game();
                    removeGameBeacon.Title = package.Message;
                    _usersAndCatalogueManager.RemoveGame(_userLogged, removeGameBeacon);
                    _communicator.SendMessageAsync(CommandConstants.RemoveGame, _messageLanguage.GameRemovedCorrectly);
                    _localSender.ExecuteAsync(_userLogged.Name, package.Message, "GameRemoval", _messageLanguage.GameRemovedCorrectly);
                }
                catch (UserNotOwnerofGame)
                {
                    _communicator.SendMessageAsync(CommandConstants.RemoveGame, _messageLanguage.UserNotGameOwner);
                    _localSender.ExecuteAsync(_userLogged.Name, package.Message, "GameRemoval", _messageLanguage.UserNotGameOwner);                    
                }
            }
            else
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void ModifyGame(CommunicatorPackage package)
        {
            if (_userLogged != null)
            {
                string[] data = new string[5];
                data = package.Message.Split("#");
                string oldTitle = data[0];
                string newTitle = data[1];
                string newGenre = data[2];
                string newSynopsis = data[3];
                string newAgeRating = data[4];
                string newCover = "";
                _communicator.SendMessageAsync(CommandConstants.SendCover, _messageLanguage.ModifyCover);

                CommunicatorPackage modifyCoverOption = _communicator.ReceiveMessageAsync().Result;
                if (modifyCoverOption.Command == CommandConstants.ReceiveCover)
                {
                    ISettingsManager settingsManager = new PathsConfiguration();
                    string path = settingsManager.ReadSetting("CoversPath");
                    newCover = _communicator.ReceiveFileAsync(path).Result;
                }

                Game newGame = new Game(newTitle, newCover, newGenre, newSynopsis, newAgeRating);
                Game oldGameBeacon = new Game();
                oldGameBeacon.Title = oldTitle;
                try
                {
                    _usersAndCatalogueManager.ModifyGame(_userLogged, oldGameBeacon, newGame);
                    _communicator.SendMessageAsync(CommandConstants.ModifyGame, _messageLanguage.GameModifiedCorrectly);
                    _localSender.ExecuteAsync(_userLogged.Name, oldTitle, "GameModification", _messageLanguage.GameModifiedCorrectly);   
                }
                catch (UserNotOwnerofGame)
                {
                    _communicator.SendMessageAsync(CommandConstants.ModifyGame, _messageLanguage.UserNotGameOwner);
                    _localSender.ExecuteAsync(_userLogged.Name, oldTitle, "GameModification", _messageLanguage.UserNotGameOwner);
                }
            }
            else
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void PublishQualification(CommunicatorPackage package)
        {
            if (_userLogged != null)
            {
                string[] data = new string[3];
                data = package.Message.Split("#");
                string gameName = data[0];
                int stars = Int32.Parse(data[1]);
                string comment = data[2];

                Qualification q = new Qualification();
                q.Comment = comment;
                q.Stars = stars;
                q.User = _userLogged.Name;
                string messageToReturn;
                try
                {
                    _usersAndCatalogueManager.AddCommunityQualification(gameName, q);
                    messageToReturn=_messageLanguage.QualificationAdded;
                }
                catch (GameNotFound)
                {
                   messageToReturn= _messageLanguage.GameNotFound;
                }
                _communicator.SendMessageAsync(CommandConstants.PublishQualification, messageToReturn);
                _localSender.ExecuteAsync(_userLogged.Name, gameName, "PublishQualification", messageToReturn);   
            }
            else
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void ViewGameDetails(CommunicatorPackage package)
        {
            try
            {
                Game game = _usersAndCatalogueManager.GetGame(package.Message);
                GameDetails gameDetails = new GameDetails(game);
                string toReturn = gameDetails.DetailsOnstring();
                _communicator.SendMessageAsync(CommandConstants.GameDetails, toReturn);
                _localSender.ExecuteAsync(_userLogged.Name, package.Message, "ViewDetails", toReturn);
            }
            catch (GameNotFound)
            {
                _communicator.SendMessageAsync(CommandConstants.GameNotExits, _messageLanguage.GameNotFound);
                _localSender.ExecuteAsync(_userLogged.Name, package.Message, "ViewDetails", _messageLanguage.GameNotFound);
            }
        }

        private void SendCover(CommunicatorPackage package)
        {
            Game game = _usersAndCatalogueManager.GetGame(package.Message);
            string path = game.Cover;
            try
            {
                _communicator.SendFileAsync(path);
            }
            catch (System.IO.FileNotFoundException)
            {
                FileDefaultCoverCreation cover = new FileDefaultCoverCreation();
                Bitmap defaultImage = cover.DrawFilledRectangle(1024, 1024);
                defaultImage.Save(_pathsManager.ReadSetting("DefaultPath"));
                _communicator.SendFileAsync(_pathsManager.ReadSetting("DefaultPath"));
            }
        }

        private void MyGames()
        {
            if (_userLogged != null)
            {
                string toReturn = _userLogged.GetMyGames();
                _communicator.SendMessageAsync(CommandConstants.MyGames, toReturn);
                _localSender.ExecuteAsync(_userLogged.Name, "", "ViewMyGames", toReturn);
            }
            else
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
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
            catch(FormatException){}

            try
            {
                string games = _usersAndCatalogueManager.SearchGames(title, genre, stars);
                _communicator.SendMessageAsync(CommandConstants.SearchGame, games);
                _localSender.ExecuteAsync(_userLogged.Name, "", "SearchGame", games);
            }
            catch (GameNotFound)
            {
                _communicator.SendMessageAsync(CommandConstants.GameNotExits, _messageLanguage.GameNotFound);
                _localSender.ExecuteAsync(_userLogged.Name, "", "SearchGame", _messageLanguage.GameNotFound);
            }
        }

        private void StartUpMenu()
        {
            string messageToSend = _messageLanguage.StartUpMessage;
            _communicator.SendMessageAsync(CommandConstants.StartupMenu, messageToSend);
        }

        private void UserRegistration(CommunicatorPackage received)
        {
            string userName = received.Message;
            try
            {
                _usersAndCatalogueManager.ContainsUser(userName);
                _communicator.SendMessageAsync(CommandConstants.RegisterUser, _messageLanguage.UserRepeated);
                _localSender.ExecuteAsync(userName, "", "UserRegistration", _messageLanguage.UserRepeated);
            }
            catch (UserNotFound)
            {
                _usersAndCatalogueManager.AddUser(userName);
                _communicator.SendMessageAsync(CommandConstants.RegisterUser,
                    _messageLanguage.UserCreated + _usersAndCatalogueManager.Users.Count + "\n");
                _localSender.ExecuteAsync(userName, "", "UserRegistration", _messageLanguage.UserCreated + _usersAndCatalogueManager.Users.Count);
            }
        }

        private void UserLogin(CommunicatorPackage received)
        {
            string user = received.Message;
            try
            {
                _userLogged = _usersAndCatalogueManager.Login(user);
                _communicator.SendMessageAsync(CommandConstants.userLogged, _messageLanguage.UserLogged);
                _localSender.ExecuteAsync(user, "", "UserLogIn", _messageLanguage.UserLogged);
            }
            catch (UserNotFound)
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserIncorrect);
                _localSender.ExecuteAsync(user, "", "UserLogIn", _messageLanguage.UserIncorrect);
            }
            catch (UserAlreadyLoggedIn)
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserAlreadyLoggedIn);
                _localSender.ExecuteAsync(user, "", "UserLogIn", _messageLanguage.UserAlreadyLoggedIn);
            }
        }

        private void BuyGame(CommunicatorPackage package)
        {
            if (_userLogged != null)
            {
                try
                {
                    Game game = _usersAndCatalogueManager.GetGame(package.Message);
                    _userLogged.BuyGame(game.Title);
                    _communicator.SendMessageAsync(CommandConstants.buyGame, _messageLanguage.GamePurchased);
                    _localSender.ExecuteAsync(_userLogged.Name, package.Message, "BuyGame", _messageLanguage.GamePurchased);
                }
                catch (GameNotFound)
                {
                    _communicator.SendMessageAsync(CommandConstants.buyGame, _messageLanguage.GameNotFound);
                    _localSender.ExecuteAsync(_userLogged.Name, package.Message, "BuyGame", _messageLanguage.GameNotFound);
                }
                catch (GameAlreadyPurchased)
                {
                    _communicator.SendMessageAsync(CommandConstants.buyGame, _messageLanguage.GameAlreadyInLibrary);
                    _localSender.ExecuteAsync(_userLogged.Name, package.Message, "BuyGame", _messageLanguage.GameAlreadyInLibrary);
                }
            }
            else
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        private void ShowCatalogue()
        {
            string messageToSend = _usersAndCatalogueManager.GetCatalogue();

            if (messageToSend.Equals(""))
            {
                messageToSend = _messageLanguage.EmptyCatalogue;
            }

            _communicator.SendMessageAsync(CommandConstants.ViewCatalogue, _messageLanguage.CatalogueView + messageToSend);
            _localSender.ExecuteAsync(_userLogged.Name, "", "ShowCatalogue", "");
        }

        private void AddGame(CommunicatorPackage package)
        {
            if (_userLogged != null)
            {
                string[] data = new string[4];
                data = package.Message.Split("#");
                string title = data[0];
                string genre = data[1];
                string synopsis = data[2];
                string ageRating = data[3];
                _communicator.SendMessageAsync(CommandConstants.SendCover, _messageLanguage.SendGameCover);
                bool okReceived = false;
                string coverPath = "Default";
                while (!okReceived)
                {
                    try
                    {
                        string path = _pathsManager.ReadSetting("CoversPath");
                        coverPath = _communicator.ReceiveFileAsync(path).Result;
                        Console.WriteLine(coverPath);
                        okReceived = true;
                    }
                    catch (Exception)
                    {
                        _communicator.SendMessageAsync(CommandConstants.SendCover, _messageLanguage.ErrorGameCover);
                    }
                }

                Game gameToAdd = new Game(title, coverPath, genre, synopsis, ageRating);

                _usersAndCatalogueManager.AddGame(_userLogged, gameToAdd);

                _communicator.SendMessageAsync(CommandConstants.AddGame, _messageLanguage.GameAdded);
                _localSender.ExecuteAsync(_userLogged.Name, title, "AddGame", _messageLanguage.GameAdded);
            }
            else
            {
                _communicator.SendMessageAsync(CommandConstants.userNotLogged, _messageLanguage.UserNotLogged);
            }
        }

        public void Listen()
        {
            try
            {
                Task<CommunicatorPackage> taskPackage = this._communicator.ReceiveMessageAsync();
                this.MessageInterpreter(taskPackage.Result);
            }
            catch (ClientClosingException)
            {
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