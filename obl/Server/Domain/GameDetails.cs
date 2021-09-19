using System.Collections;
using System.Collections.ObjectModel;

namespace Server.Domain
{
    public class GameDetails
    {
        public Game Game { get; set; }
        
        public Collection<Qualification> Reviews { get; set; }

        public int AverageMark { get; set; }
        
        public  GameDetails(Game game)
        {
            this.Game = game;
            this.Reviews = game.CommunityQualifications;
            if (game.CommunityQualifications.Count != 0)
            {
                this.AverageMark = CalculateAverageMark(game);
            }
            else this.AverageMark = 0;
        }

        private int CalculateAverageMark(Game game)
        {
            int totalStars = 0;
            Collection<Qualification> qualifications = game.CommunityQualifications;
            for (int i = 0; i < qualifications.Count; i++)
            {
                totalStars += qualifications[i].Stars;
            }

            return totalStars / qualifications.Count;
        }

        public string DetailsOnstring()
        {
            string ret = "";
            ret += $"Detalles: \n" +
                   $"titulo: {this.Game.Title} \n " +
                   $"genero: {this.Game.Genre} \n " +
                   $"sinopsis: {this.Game.Synopsis} \n " +
                   $"clasificacion de edad:{this.Game.AgeRating} \n" +
                   $"promedio de estrellas {this.Game.Stars} \n"  +
                   $"comentarios: \n";
            foreach (var qualification in this.Game.CommunityQualifications)
            {
                string qual = $"usuario: {qualification.User} \n" +
                              $"comentario: {qualification.comment} \n " +
                              $"estrellas: {qualification.Stars} \n \n ";

                ret += qual;
            }

            return ret;
        }
    }
}