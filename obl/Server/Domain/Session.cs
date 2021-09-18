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
                    _communicator.SendMessage(CommandConstants.StartupMenu, messageReturn);
                    break;
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

            if (_usersAndCatalogueManager.ContainsUser(userName))
                _communicator.SendMessage(CommandConstants.RegisterUser, _messageLanguage.UserRepeated);
                
            else
            {
                _usersAndCatalogueManager.AddUser(userName);
                _communicator.SendMessage(CommandConstants.RegisterUser, _messageLanguage.UserCreated + _usersAndCatalogueManager.Users.Count + "\n" + _messageLanguage.BackToStartUpMenu);
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
                _communicator.SendMessage(CommandConstants.LoginUser, _messageLanguage.UserIncorrect);
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

        private void AddGame()
        {
            if (_userLogged != null)
            {
                try
                {
                    // SendMessage(_messageLanguage.NewGameInit);
                    // string title = ReceiveMessage();
                    // SendMessage(_messageLanguage.GameGenre);
                    // string genre = ReceiveMessage();
                    // SendMessage(_messageLanguage.GameSynopsis);
                    // string synopsis = ReceiveMessage();     
                    // SendMessage(_messageLanguage.GameAgeRestriction);
                    // string ageRating = ReceiveMessage();
                    // SendMessage(_messageLanguage.GameCover);
                    // string coverPath = ReceiveMessage();

                    // Game gameToAdd = new Game(title, coverPath, genre, synopsis, ageRating);
                    // _usersAndCatalogueManager.AddGame(gameToAdd);

                    // SendMessage(_messageLanguage.GameAdded);
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