using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConsoleAppSocketServer.Domain
{
    public class Catalogue
    {
        public Collection<Game> Games { get; set; }

        public Catalogue()
        {
            this.Games = new Collection<Game>();
        }


        public Collection<Game>  SearchGame(string title, string genre, int qualification)
        {
            Collection<Game> matchedGames = new Collection<Game>();
            if(!title.Equals("")) matchedGames.Add(SearchGameByTitle(title));
            else
            {
                if (!genre.Equals(""))
                {
                    Collection<Game> gamesByGenre = SearchGameByGenre(genre);
                    matchedGames.Concat(gamesByGenre);
                }
                if (qualification!=-1) //-1 qualification does not exists
                {
                    Collection<Game> gamesByCualification = SearchGameByQualification(qualification);
                    
                    if (matchedGames.Count != 0) //if it was also searched by gender
                    {
                        matchedGames=  (Collection<Game>) matchedGames.Intersect(gamesByCualification); //intersects matching games by genre and by qualification
                    }
                    else
                    {// add only games by qualification
                        matchedGames.Concat(gamesByCualification);
                    }
                }
               
            }
            return matchedGames;
        }
        
        public Collection<Game> Show()
        {
            return this.Games;
        }

        public string ShowGamesOnStringList()
        {
            string ret = "";
            if (Games.Count == 0)
            {
                ret = SystemMessages.EmptyCatalogue;
            }
            else
            {
                ret = "lista de juegos: \n \n";
                foreach(Game game in Games)
                {
                    ret += $"{game.Title} \n";
                }
            }
            return ret;
        }

        public GameDetails ShowDetails(Game game)
        {
            GameDetails gameDetails = new GameDetails(game);
            return gameDetails;
        }

        public void AddGame(Game gameToAdd)
        {
            this.Games.Add(gameToAdd);
        }
        
        private Collection<Game> SearchGameByGenre(string genre)
        {
            Collection<Game> matchingGames = new Collection<Game>();
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Genre.Equals(genre)) matchingGames.Add(this.Games[i]);
            }

            if (matchingGames.Count == 0) throw new Exception("there are no games of this genre");
            
            return matchingGames;
        }

        private Game SearchGameByTitle(string title)
        {
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Title.Equals(title)) return this.Games[i];
            }

            throw new Exception("Game not found");
        }

        private Collection<Game> SearchGameByQualification(int qualification)
        {
            Collection<Game> matchingGames = new Collection<Game>();
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Stars==(qualification)) matchingGames.Add(this.Games[i]);
            }
            if (matchingGames.Count == 0) throw new Exception("there are no games of this qualification");
            return matchingGames;
        }
    }
}