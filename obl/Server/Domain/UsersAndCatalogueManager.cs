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

        public UsersAndCatalogueManager() 
        {
            this.Users = new Collection<User>();
            this.Catalogue = new Catalogue();
        }

        public void AddUser(string username)
        {
            User user = new User();
            user.Name = username;
            this.Users.Add(user);
        }

        public User GetUser(string userId)
        {
            foreach (User user in Users)
            {
                if (user.Name == userId) return user;
            }
            throw new Exception("User does not exists");
        }

        public Catalogue GetCatalogue()
        {
            return this.Catalogue;
        }

        public bool ContainsUser(string userName)
        {
            User userToCompare = new User();
            userToCompare.Name = userName;
            return Users.Contains(userToCompare);
        }

        public bool Login(string user)
        {
            return ContainsUser(user);
        }

        public void AddGame(User publisher, Game gameToAdd)
        {
            this.Catalogue.AddGame(gameToAdd);
            publisher.CreateGame(gameToAdd);
        }

        public bool ExistsGame(Game gameToAdd)
        {
            bool toReturn = false;
            if(this.Catalogue.ExistsGame(gameToAdd))
                toReturn = true;
            return toReturn;
        }

        public void RemoveGame(User publisher, Game gameToRemove)
        {
            publisher.RemoveFromPublishedGames(gameToRemove);

            foreach (User user in this.Users)
            {
                user.RemoveFromAcquiredGames(gameToRemove);
            }

            this.Catalogue.DeleteGame(gameToRemove.Title);
        }

        public void ModifyGame(User publisher, Game oldGame, Game newGame)
        {
            publisher.IsOwner(oldGame);
            Game newGameFullData = this.Catalogue.ModifyGame(oldGame.Title, newGame);
            publisher.ModifyGameForOwner(oldGame, newGameFullData);
            foreach (User user in this.Users)
            {
                user.ModifyGameForNotOwner(oldGame, newGameFullData);
            }
        }
    }
}