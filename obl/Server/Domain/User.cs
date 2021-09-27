using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Server.Domain.ServerExceptions;

namespace Server.Domain
{
    public class User
    {
        public string Name { get; set; }
        public bool loggedIn {get; set;}
        public ArrayList AcquireGames { get; set; }
        public ArrayList PublishedGames { get; set; }


        public User()
        {
            this.AcquireGames = new ArrayList();
            this.PublishedGames = new ArrayList();
            loggedIn = false;
        }

        public void LogIn()
        {
            if(!loggedIn)
            {
                this.loggedIn = true;
            }
            else
            {
                throw new UserAlreadyLoggedIn();
            }
        }

        public void LogOut()
        {
            this.loggedIn = false;
        }

        public void BuyGame(string boughtGameName)
        {
            if (AcquireGames.Contains(boughtGameName))
            {
                throw new GameAlreadyPurchased();
            }
            else
            {
                this.AcquireGames.Add(boughtGameName);
            }
        }

        public void CreateGame(string newGameName)
        {
            this.PublishedGames.Add(newGameName);
        }

        public void RemoveFromAcquiredGames(string gameNameToRemove)
        {
            if (AcquireGames.Contains(gameNameToRemove))
            {
                AcquireGames.Remove(gameNameToRemove);
            }
        }

        public void RemoveFromPublishedGames(string gameNameToRemove)
        {
            IsOwner(gameNameToRemove);
            PublishedGames.Remove(gameNameToRemove);
        }

        public void ModifyGameForOwner(string oldGameName, string NewGameName)
        {
            if(AcquireGames.Contains(oldGameName))
            {
                this.AcquireGames.Remove(oldGameName);
                this.AcquireGames.Add(NewGameName);
            }

            this.PublishedGames.Remove(oldGameName);
            this.PublishedGames.Add(NewGameName);
        }

        public void ModifyGameForNotOwner(string oldGameName, string NewGameName)
        {
            if(AcquireGames.Contains(oldGameName))
            {
                this.AcquireGames.Remove(oldGameName);
                this.AcquireGames.Add(NewGameName);
            }
        }

        public void IsOwner(string GameName)
        {
            if(!PublishedGames.Contains(GameName))
            {
                throw new UserNotOwnerofGame();
            }
        }

        public string GetMyGames()
        {
            string ret = "";
            foreach (var game in AcquireGames)
            {
                ret += $"{game} \n";
            }

            return ret;
        }

        public override bool Equals(object? obj)
        {
            User user = (User) obj;
            return this.Name.Equals(user.Name);
        }
    }
}