using System;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic;
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

        public Collection<User>  Users { get; set; }
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
            lock(_userCollectionLock)
            {
                User user = new User();
                user.Name = username;
                this.Users.Add(user);
            }
        }

        public void ContainsUser(string userName)
        {
            User userToCompare = new User();
            userToCompare.Name = userName;
            
            lock(_userCollectionLock)
            {
                if(!Users.Contains(userToCompare))
                {
                    throw new UserNotFound();
                }
            }
        }

        public User Login(string userName)
        {
            ContainsUser(userName);

            User userToReturn = new User();
            lock(_userCollectionLock)
            {
                foreach (User user in Users)
                {
                    if (user.Name == userName) userToReturn = user;
                }
            }
            return userToReturn;
        }

        public Catalogue GetCatalogue()
        {
            Catalogue copyCatalogue = new Catalogue();
            lock(_catalogueLock)
            {
                copyCatalogue = this.Catalogue;
            }
            return copyCatalogue;
        }

        public void AddGame(User publisher, Game gameToAdd)
        {
            lock(_catalogueLock)
            {
                this.Catalogue.AddGame(gameToAdd);
            }
            publisher.CreateGame(gameToAdd.Title);
        }

        public bool ExistsGame(Game gameToAdd)
        {
            bool toReturn = false;
            lock(_catalogueLock)
            {
                if(this.Catalogue.ExistsGame(gameToAdd))
                    toReturn = true;
            }
            return toReturn;
        }

        public void RemoveGame(User publisher, Game gameToRemove)
        {
            publisher.RemoveFromPublishedGames(gameToRemove.Title);

            lock(_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    user.RemoveFromAcquiredGames(gameToRemove.Title);
                }
            }

            lock(_catalogueLock)
            {
                this.Catalogue.DeleteGame(gameToRemove.Title);
            }
        }

        public void ModifyGame(User publisher, Game oldGame, Game newGame)
        {
            publisher.IsOwner(oldGame.Title);

            Game newGameFullData = new Game();
            lock(_catalogueLock)
            {
                newGameFullData = this.Catalogue.ModifyGame(oldGame.Title, newGame);
            }

            publisher.ModifyGameForOwner(oldGame.Title, newGameFullData.Title);

            lock(_userCollectionLock)
            {
                foreach (User user in this.Users)
                {
                    user.ModifyGameForNotOwner(oldGame.Title, newGameFullData.Title);
                }
            }
        }
    }
}