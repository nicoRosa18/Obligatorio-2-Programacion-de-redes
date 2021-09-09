using System;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic;

namespace ConsoleAppSocketServer.Domain
{

    public class UsersAndCatalogueManager
    {
        public Collection<User>  Users { get; set; }
        
        public Catalogue Catalogue { get; set; }

        public UsersAndCatalogueManager() //implement singleton
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
    }
}