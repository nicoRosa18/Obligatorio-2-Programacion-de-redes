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

        public void ModifyGameByAdmin(Game oldGame, Game newGame)
        {
            Game newGameFullData = new Game();
            lock (_catalogueLock)
            {
                newGameFullData = this.Catalogue.ModifyGame(oldGame.Title, newGame);
            }

            lock (_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    user.ModifyGameForNotOwner(oldGame.Title, newGameFullData.Title);
                }
            }
        }

        public void RemoveGameByAdmin(Game gameToRemove)
        {
            RemoveFromOwner(gameToRemove);

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

        public void ModifyUser(string oldName, string newName)
        {
            try
            {
                ContainsUser(newName);
                throw new Exception("new user name already exists");
            }
            catch
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


        public void DeleteUser(User userToDelete)
        {
            lock (_userCollectionLock)
            {
                foreach (User user in Users)
                {
                    if (user.Name.Equals(userToDelete.Name))
                    {
                        Users.Remove(user);
                        break;
                    }
                }
            }
        }

        public void AsociateGameToUser(Game game, User user)
        {
            lock (_userCollectionLock)
            {
                lock (_catalogueLock)
                {
                    user.BuyGame(game.Title);
                }
            }
        }


        public void DesaciociateGameToUser(Game game, User user)
        {
            lock (_userCollectionLock)
            {
                lock (_catalogueLock)
                {
                    user.RemoveFromAcquiredGames(game.Title);
                }
            }
        }

        private void RemoveFromOwner(Game game)
        {
            lock (_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    if (user.PublishedGames.Contains(game))
                    {
                        user.RemoveFromPublishedGames(game.Title);
                    }
                }
            }
        }
    }
}