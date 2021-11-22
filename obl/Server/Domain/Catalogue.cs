using System.Collections.Generic;
using System.Collections.ObjectModel;
using Server.Domain.ServerExceptions;

namespace Server.Domain
{
    public class Catalogue
    {
        public const int NOQUALIFICATION = -1;
        public const string NOTITLE = "";
        public const string NOGENRE = "";
        public Collection<Game> Games { get; set; }

        public Catalogue()
        {
            this.Games = new Collection<Game>();
        }

        public bool ExistsGame(Game gameToVerify)
        {
            foreach (Game game in Games)
            {
                if (game.Title.Equals(gameToVerify.Title))
                    throw new GameAlreadyExists();
            }

            return false;
        }

        public string SearchGame(string title, string genre, int qualification)
        {
            List<Game> matchedGames = new List<Game>();
            if (!title.Equals(NOTITLE)) matchedGames.Add(SearchGameByTitle(title));
            else
            {
                if (!genre.Equals(NOGENRE))
                {
                    List<Game> gamesByGenre = SearchGameByGenre(genre);
                    matchedGames.AddRange(gamesByGenre);
                }

                if (qualification != NOQUALIFICATION)
                {
                    List<Game> gamesByCualification = SearchGameByQualification(qualification);

                    if (matchedGames.Count != 0)
                    {
                        matchedGames = Intersect(matchedGames, gamesByCualification);
                    }
                    else
                    {
                        if (genre.Equals(NOGENRE))
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
            game.AddCommunityQualification(newQualification);
        }

        public void DeleteGame(string gameName)
        {
            Game gameToDelete = SearchGameByTitle(gameName);

            this.Games.Remove(gameToDelete);
        }

        public Game ModifyGame(string oldGameTitle, Game newGame)
        {
            Game gameToModify = GetGameByReference(oldGameTitle);

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

        private List<Game> SearchGameByGenre(string genre)
        {
            List<Game> matchingGames = new List<Game>();
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Genre.Equals(genre)) matchingGames.Add(this.Games[i].GameCopy());
            }

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

        private List<Game> SearchGameByQualification(int qualification)
        {
            List<Game> matchingGames = new List<Game>();
            for (int i = 0; i < this.Games.Count; i++)
            {
                if (this.Games[i].Stars == (qualification)) matchingGames.Add(this.Games[i].GameCopy());
            }

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

        private List<Game> Intersect(List<Game> collection1, List<Game> collection2)
        {
            List<Game> games = new List<Game>();
            foreach (var game in collection1)
            {
                if (collection2.Contains(game)) games.Add(game);
            }

            return games;
        }
    }
}