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
                   $" Titulo: {this.Game.Title} \n" +
                   $" Genero: {this.Game.Genre} \n" +
                   $" Sinopsis: {this.Game.Synopsis} \n" +
                   $" Clasificacion de edad:{this.Game.AgeRating} \n" +
                   $" Promedio de estrellas {this.Game.Stars} \n"  +
                   $"Comentarios: \n";
            foreach (var qualification in this.Game.CommunityQualifications)
            {
                string qual = $" Usuario: {qualification.User} \n" +
                              $" Comentario: {qualification.comment} \n" +
                              $" Estrellas: {qualification.Stars} \n \n";

                ret += qual;
            }

            return ret;
        }
    }
}