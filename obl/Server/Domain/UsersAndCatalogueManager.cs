using System;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic;

namespace ConsoleAppSocketServer.Domain
{
    public class UsersAndCatalogueManager
    {
        private static UsersAndCatalogueManager _instance;
        public static UsersAndCatalogueManager Instance 
        { 
            get 
            {
                return _instance;
            } 
        }

        public Collection<User>  Users { get; set; }
        
        public Catalogue Catalogue { get; set; }

        public UsersAndCatalogueManager() 
        {
            CheckSingleInstanceOfSingleton();

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

        public void AddGame(Game gameToAdd)
        {
            this.Catalogue.AddGame(gameToAdd);
        }

        private void CheckSingleInstanceOfSingleton()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }
    }
}