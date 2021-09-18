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
            this.AverageMark = CalculateAverageMark(game);
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
    }
}