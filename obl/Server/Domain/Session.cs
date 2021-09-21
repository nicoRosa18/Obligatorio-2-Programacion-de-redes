using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using Common.Protocol;
using Server.Domain.ServerExceptions;
using Common.Communicator.Exceptions;
using Common.Communicator;

namespace Server.Domain
{
    public class Session
    {
        public bool Active { get; set; }

        public static UsersAndCatalogueManager _usersAndCatalogueManager { get; set; }

        private User _userLogged { get; set; }

        private ICommunicator _communicator { get; set; }

        private Message _messageLanguage { get; set; }

        public Session(ICommunicator communicator, Message messageLanguage)
        {
            this.Active = true;
            this._communicator = communicator;
            this._messageLanguage = messageLanguage;
        }

        public void MessageInterpreter(CommunicatorPackage package)
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
                     
                // case "":
                //     CloseConnection();
                //     break;
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
                Game game = _usersAndCatalogueManager.Catalogue.GetGameByName(package.Message);
                _communicator.SendMessage(CommandConstants.GameExists,"");
            }
            catch (Exception e)
            {
                _communicator.SendMessage(CommandConstants.GameNotExits,"");
            }
        }

        private void PublishQualification(CommunicatorPackage package)
        {
            string[] data = new string[3];
            data = package.Message.Split("#");
            string gameName = data[0];
            int stars= Int32.Parse(data[1]);
            string comment = data[2];
            Game game = _usersAndCatalogueManager.Catalogue.GetGameByName(gameName);
            Qualification q = new Qualification();
            q.comment = comment;
            q.Stars = stars;
            q.User = _userLogged.Name;
            q.game = game;
            game.AddCommunityQualification(q);
            _communicator.SendMessage(CommandConstants.PublishQualification, _messageLanguage.QualificationAdded);
        }

        private void ViewGameDetails(CommunicatorPackage package)
        {
            try
            {
                Catalogue catalogue = _usersAndCatalogueManager.Catalogue;
                Game game = catalogue.GetGameByName(package.Message);
                GameDetails gameDetails = new GameDetails(game);
                _communicator.SendMessage(CommandConstants.GameDetails, gameDetails.DetailsOnstring());
            }
            catch (Exception e)
            {
                _communicator.SendMessage(package.Command,e.Message);
            }
        }

        private void MyGames()
        {
            _communicator.SendMessage( CommandConstants.MyGames,_userLogged.GetMyGames());
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
            catch {}
            string games= _usersAndCatalogueManager.Catalogue.SearchGame(title, genre, stars);
            
            _communicator.SendMessage(CommandConstants.SearchGame,games);
        }

        private void StartUpMenu()
        {
            string messageToSend = _messageLanguage.StartUpMessage;
            _communicator.SendMessage(CommandConstants.StartupMenu, messageToSend);
        }
        
        private void UserRegistration(CommunicatorPackage received)
        {
            string userName = received.Message;
            if (_usersAndCatalogueManager.ContainsUser(userName))
                _communicator.SendMessage(CommandConstants.RegisterUser, _messageLanguage.UserRepeated);
                
            else
            {
                _usersAndCatalogueManager.AddUser(userName);
                _communicator.SendMessage(CommandConstants.RegisterUser, _messageLanguage.UserCreated + _usersAndCatalogueManager.Users.Count + "\n");
            }            
        }

        private void UserLogin(CommunicatorPackage received)
        {
            string user = received.Message;
            if (_usersAndCatalogueManager.Login(user))
            {
                _userLogged = _usersAndCatalogueManager.GetUser(user);
                mainMenu();
            }
            else
            {
                _communicator.SendMessage(CommandConstants.userNotLogged, _messageLanguage.UserIncorrect);
            }
        }

        private void BuyGame(CommunicatorPackage package)
        {
            try
            {
                Catalogue catalogue = _usersAndCatalogueManager.GetCatalogue();
                string gameName = package.Message;
                Game game = catalogue.GetGameByName(gameName);
                _userLogged.BuyGame(game);
                _communicator.SendMessage(CommandConstants.buyGame, _messageLanguage.GamePurchased);
            }
            catch  (Exception e)
            {
                _communicator.SendMessage(CommandConstants.buyGame, e.Message);
            }
        }

        private void ShowCatalogue()
        {
            // if (_userLogged != null)
            // {
            //     Catalogue catalogue = _usersAndCatalogueManager.GetCatalogue();

            //     string messageToSend = catalogue.ShowGamesOnStringList();
            //     if(messageToSend.Equals("")){
            //         messageToSend = _messageLanguage.EmptyCatalogue;
            //     }
            //     SendMessage(_messageLanguage.CatalogueView + messageToSend);
            // }
            // else
            // {
            //     SendMessage(_messageLanguage.InvalidOption);
            // }
        }

        private void mainMenu()
        {
            if (_userLogged != null)
            {
                string messageToSend = _messageLanguage.MainMenuMessage;
                _communicator.SendMessage(CommandConstants.MainMenu, messageToSend);
            }
            else
            {
                _communicator.SendMessage(CommandConstants.MainMenu, _messageLanguage.InvalidOption);
            }
        }

        private void AddGame(CommunicatorPackage package)
        {
            if (_userLogged != null)
            {
                try
                {
                    string[] data= new string[4];
                    data = package.Message.Split("#");
                    string title = data[0];
                    string genre = data[1];
                    string synopsis =data[2];     
                    string ageRating = data[3];
                    _communicator.SendMessage(CommandConstants.SendCover,_messageLanguage.SendGameCover);
                     //aca envio un string pidieendo la caratula
                     //aca la recibo
                    string coverPath = "Default";
                    try
                    {
                       coverPath = _communicator.ReceiveFile();
                    }
                    catch(Exception e)
                    {
                       Console.WriteLine(e.Message);
                    }
                    Game gameToAdd = new Game(title, coverPath, genre, synopsis, ageRating);
                    _usersAndCatalogueManager.AddGame(gameToAdd);
                    _communicator.SendMessage(CommandConstants.AddGame,_messageLanguage.SendGameCover);
                }
                catch (Exception e) //to be implemented
                {
                    _communicator.SendMessage(CommandConstants.Message, e.Message);
                }
            }
            else
            {
                _communicator.SendMessage(CommandConstants.Message, _messageLanguage.InvalidOption);
            }
        }

        public void Listen()
        {  
            try
            {
                this.MessageInterpreter(this._communicator.ReceiveMessage());
            }
            catch (ClientClosingException e)
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
        }
               
    }
}