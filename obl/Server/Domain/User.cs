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
        public ICollection<Game> AcquireGames { get; set; }
        public ArrayList PublishedGames { get; set; }
        
        public void PublishGame(Game game)
        {
            this.PublishedGames.Add(game);
        }

        public User()
        {
            this.AcquireGames = new Collection<Game>();
            this.PublishedGames = new ArrayList();
        }

        public void PublishQualification(Qualification qualification)
        {
            qualification.game.AddCommunityQualification(qualification);
        }

        public void BuyGame(Game game)
        {
            if (AcquireGames.Contains(game))
            {
                throw new GameAlreadyPurchased();
            }
            else
            {
                this.AcquireGames.Add(game);
            }
        }

        public void CreateGame(Game gameCreated)
        {
            this.PublishedGames.Add(gameCreated);
        }

        public void RemoveFromAcquiredGames(Game gameToRemove)
        {
            if (AcquireGames.Contains(gameToRemove))
            {
                AcquireGames.Remove(gameToRemove);
            }
        }

        public void RemoveFromPublishedGames(Game gameNameToRemove)
        {
            IsOwner(gameNameToRemove);
            PublishedGames.Remove(gameNameToRemove);
        }

        public void ModifyGameForOwner(Game oldGame, Game NewGame)
        {
            IsOwner(oldGame);
            if(this.AcquireGames.Remove(oldGame))
            {
                this.AcquireGames.Add(NewGame);
            }
            this.PublishedGames.Remove(oldGame);
            this.PublishedGames.Add(NewGame);
        }

        public void ModifyGameForNotOwner(Game oldGame, Game NewGame)
        {
            if(this.AcquireGames.Remove(oldGame))
            {
                this.AcquireGames.Add(NewGame);
            }
        }

        public void IsOwner(Game GameName)
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
                ret += $"{game.Title} \n";
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