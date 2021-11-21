using System;
using System.Collections.ObjectModel;
using Server.Domain.ServerExceptions;

namespace Server.Domain
{
    public class UsersAndCatalogueManager
    {
        private static UsersAndCatalogueManager _instance;
        private static readonly object padlock = new object();

        public static UsersAndCatalogueManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new UsersAndCatalogueManager();
                    }

                    return _instance;
                }
            }
        }

        public Collection<User> Users { get; set; }
        public Catalogue Catalogue { get; set; }
        private readonly object _userCollectionLock = new object();
        private readonly object _catalogueLock = new object();

        public UsersAndCatalogueManager()
        {
            this.Users = new Collection<User>();
            this.Catalogue = new Catalogue();
        }

        public void AddUser(string username)
        {
            User user = new User();
            user.Name = username;
            lock (_userCollectionLock)
            {
                this.Users.Add(user);
            }
        }

        public void ContainsUser(string userName)
        {
            User userToCompare = new User();
            userToCompare.Name = userName;

            lock (_userCollectionLock)
            {
                if (!Users.Contains(userToCompare))
                {
                    throw new UserNotFound();
                }
            }
        }

        public User Login(string userName)
        {
            ContainsUser(userName);

            User userToReturn = new User();
            lock (_userCollectionLock)
            {
                foreach (User user in Users)
                {
                    if (user.Name == userName)
                    {
                        user.LogIn();
                        userToReturn = user;
                    }
                }
            }

            return userToReturn;
        }

        public string GetCatalogue()
        {
            string copyCatalogue;
            lock (_catalogueLock)
            {
                copyCatalogue = this.Catalogue.ShowGamesOnStringList();
            }

            return copyCatalogue;
        }

        public Game GetGame(string gameName)
        {
            Game cleanCopyGame = new Game();
            lock (_catalogueLock)
            {
                cleanCopyGame = this.Catalogue.GetGameByNameCopy(gameName);
            }

            return cleanCopyGame;
        }

        public string SearchGames(string title, string genre, int qualification)
        {
            string SearchGamesCopy;
            lock (_catalogueLock)
            {
                SearchGamesCopy = this.Catalogue.SearchGame(title, genre, qualification);
            }

            return SearchGamesCopy;
        }

        public void AddGame(User publisher, Game gameToAdd)
        {
            lock (_catalogueLock)
            {
                this.Catalogue.AddGame(gameToAdd);
            }

            publisher.CreateGame(gameToAdd.Title);
        }

        public bool ExistsGame(Game gameToAdd)
        {
            bool toReturn = true;
            lock (_catalogueLock)
            {
                if (this.Catalogue.ExistsGame(gameToAdd))
                    toReturn = false;
            }

            return toReturn;
        }

        public void AddCommunityQualification(string gameName, Qualification newQualification)
        {
            lock (_catalogueLock)
            {
                this.Catalogue.AddGameQualification(gameName, newQualification);
            }
        }

        public void RemoveGame(User publisher, Game gameToRemove)
        {
            publisher.RemoveFromPublishedGames(gameToRemove.Title);

            lock (_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    user.RemoveFromAcquiredGames(gameToRemove.Title);
                }
            }

            lock (_catalogueLock)
            {
                this.Catalogue.DeleteGame(gameToRemove.Title);
            }
        }

        public void ModifyGame(User publisher, Game oldGame, Game newGame)
        {
            publisher.IsOwner(oldGame.Title);

            Game newGameFullData = new Game();
            lock (_catalogueLock)
            {
                newGameFullData = this.Catalogue.ModifyGame(oldGame.Title, newGame);
            }

            publisher.ModifyGameForOwner(oldGame.Title, newGameFullData.Title);

            lock (_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    user.ModifyGameForNotOwner(oldGame.Title, newGameFullData.Title);
                }
            }
        }

        public void ModifyGameByAdmin(string oldGameTitle, Game newGame)
        {
            Game newGameFullData = new Game();

            lock (_catalogueLock)
            {
                newGameFullData = this.Catalogue.ModifyGame(oldGameTitle, newGame);
            }

            lock (_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    user.ModifyGameForNotOwner(oldGameTitle, newGameFullData.Title);
                    try
                    {
                        user.IsOwner(oldGameTitle);
                        user.ModifyGameForOwner(oldGameTitle, newGame.Title);
                    }
                    catch (UserNotOwnerofGame)
                    {
                    }
                }
            }
        }

        public void RemoveGameByAdmin(string gameToRemoveTitle)
        {
            lock (_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    user.RemoveFromAcquiredGames(gameToRemoveTitle);
                    try
                    {
                        user.RemoveFromPublishedGames(gameToRemoveTitle);
                    }
                    catch (UserNotOwnerofGame)
                    {
                    }
                }
            }

            lock (_catalogueLock)
            {
                this.Catalogue.DeleteGame(gameToRemoveTitle);
            }
        }

        public void ModifyUserByAdmin(string oldName, string newName)
        {
            try
            {
                ContainsUser(newName);
                throw new UserAlreadyExists();
            }
            catch (UserNotFound)
            {
                ContainsUser(oldName);
                lock (_userCollectionLock)
                {
                    foreach (User user in Users)
                    {
                        if (user.Name.Equals(oldName))
                        {
                            user.Name = newName;
                            break;
                        }
                    }
                }
            }
        }

        public void RemoveUserByAdmin(string userNameToDelete)
        {
            lock (_userCollectionLock)
            {
                foreach (User user in Users)
                {
                    if (user.Name.Equals(userNameToDelete))
                    {
                        Users.Remove(user);
                        break;
                    }
                }
                throw new UserNotFound();
            }
        }

        public void AssociateGameToUserByAdmin(string gameTitle, string userName)
        {
            Game gameDecoy = new Game();
            gameDecoy.Title = gameTitle;
            try
            {
                lock(_catalogueLock)
                {
                    Catalogue.ExistsGame(gameDecoy);
                }
                throw new GameNotFound();
            }
            catch(GameAlreadyExists) {}
            
            lock (_userCollectionLock)
            {
                foreach (User user in Users)
                {
                    if (user.Name.Equals(userName))
                    {
                        user.BuyGame(gameTitle);
                        break;
                    }
                }
                throw new UserNotFound();
            }
        }
        
        public void DesassociateGameToUserByAdmin(string gameTitle, string userName)
        {
            Game gameDecoy = new Game();
            gameDecoy.Title = gameTitle;
            try
            {
                lock(_catalogueLock)
                {
                    Catalogue.ExistsGame(gameDecoy);
                }
                throw new GameNotFound();
            }
            catch(GameAlreadyExists) {}
            
            lock (_userCollectionLock)
            {
                foreach (User user in Users)
                {
                    if (user.Name.Equals(userName))
                    {
                        user.BuyGame(gameTitle);
                    }
                }
            }
        }
    }
}