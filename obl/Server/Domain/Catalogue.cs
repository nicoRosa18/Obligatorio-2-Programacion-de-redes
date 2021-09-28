using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Server.Domain.ServerExceptions;

namespace Server.Domain
{
    public class Catalogue
    {
        public Collection<Game> Games { get; set; }

        public Catalogue()
        {
            this.Games = new Collection<Game>();
        }

        public bool ExistsGame(Game game)
        {
            if (Games.Contains(game))
            {
                throw new GameAlreadyExists();
            }
            else
            {
                return false;
            }
        }

        public string SearchGame(string title, string genre, int qualification)
        {
            List<Game> matchedGames = new List<Game>();
            if (!title.Equals("")) matchedGames.Add(SearchGameByTitle(title));
            else
            {
                if (!genre.Equals(""))
                {
                    Collection<Game> gamesByGenre = SearchGameByGenre(genre);
                    matchedGames.AddRange(gamesByGenre);
                }

                if (qualification != -1) //-1 qualification does not exists
                {
                    Collection<Game> gamesByCualification = SearchGameByQualification(qualification);

                    if (matchedGames.Count != 0) //if it was also searched by gender
                    {
                        matchedGames =
                            (List<Game>) matchedGames.Intersect(
                                gamesByCualification); //intersects matching games by genre and by qualification
                    }
                    else
                    {
                        // add only games by qualification
                        matchedGames.AddRange(gamesByCualification);
                    }
                }
            }

            string matchedGamesOnString = ConvertToString(matchedGames);
            return matchedGamesOnString;
        }

        public Game GetGameByNameCopy(string name)
        {
            Game game = SearchGameByTitle(name);
            return game;
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
                ret = "";
            }
            else
            {
                ret = "lista de juegos: \n \n";
                for (int i = 0; i < Games.Count; i++)
                {
                    ret += $"{i}- {Games[i].Title} \n";
                }
            }

            return ret;
        }

        public void AddGame(Game gameToAdd)
        {
            this.Games.Add(gameToAdd);
        }

        public void AddGameQualification(string gameTitle, Qualification newQualification)
        {
            Game game = GetGameByReference(gameTitle);
            newQualification.Game = game;
            game.AddCommunityQualification(newQualification);
        }

        public void DeleteGame(string gameName)
        {
            Game gameToDelete = SearchGameByTitle(gameName);

            this.Games.Remove(gameToDelete);
        }

        public Game ModifyGame(string oldGameTitle, Game newGame)
        {
            Game gameToModify = SearchGameByTitle(oldGameTitle);

            if (!newGame.Title.Equals("")) gameToModify.Title = newGame.Title;
            if (!newGame.Cover.Equals("")) gameToModify.Cover = newGame.Cover;
            if (!newGame.Genre.Equals("")) gameToModify.Genre = newGame.Genre;
            if (!newGame.Synopsis.Equals("")) gameToModify.Synopsis = newGame.Synopsis;
            if (!newGame.AgeRating.Equals("")) gameToModify.AgeRating = newGame.AgeRating;

            return gameToModify;
        }

        private Game GetGameByReference(string title)
        {
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Title.Equals(title)) return this.Games[i];
            }

            throw new GameNotFound();
        }

        private Collection<Game> SearchGameByGenre(string genre)
        {
            Collection<Game> matchingGames = new Collection<Game>();
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Genre.Equals(genre)) matchingGames.Add(this.Games[i].GameCopy());
            }

            if (matchingGames.Count == 0) throw new Exception("there are no games of this genre");

            return matchingGames;
        }

        private Game SearchGameByTitle(string title)
        {
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Title.Equals(title)) return this.Games[i].GameCopy();
            }

            throw new GameNotFound();
        }

        private Collection<Game> SearchGameByQualification(int qualification)
        {
            Collection<Game> matchingGames = new Collection<Game>();
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Stars == (qualification)) matchingGames.Add(this.Games[i].GameCopy());
            }

            if (matchingGames.Count == 0) throw new Exception("there are no games of this qualification");
            return matchingGames;
        }

        private string ConvertToString(List<Game> games)
        {
            string ret = "";
            foreach (var game in games)
            {
                ret += $"{game.Title} \n";
            }

            return ret;
        }
    }
}