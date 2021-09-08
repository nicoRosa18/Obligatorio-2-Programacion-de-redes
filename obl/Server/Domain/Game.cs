using System.Collections;
using System.Collections.ObjectModel;

namespace ConsoleAppSocketServer.Domain
{
    public class Game
    {
        public string Title { get; set; }
        
        public int Cover { get; set; }  // image format 
        
        public string Genre { get; set; }
        
        public string Synopsis { get; set; }
        
        public string PublicQualification { get; set; } //this is a comment from the critics
        
        public int Stars { get; set; }

        public Collection<Qualification> ComunityQualifications { get; set; }

        public Game(string title, int cover, string genre, string synopsis, string publicQualification)
        {
            Title = title;
            Cover = cover;
            Genre = genre;
            Synopsis = synopsis;
            PublicQualification = publicQualification;
            Stars = 0;
            ComunityQualifications = new Collection<Qualification>();
        }

        public void AddComunityQualification(Qualification qualification)
        {
            this.ComunityQualifications.Add(qualification);
            UpdateStars();
        }

        private void UpdateStars()
        {
            GameDetails gameDetails = new GameDetails(this);
            Stars = gameDetails.AverageMark;
        }
    }
}