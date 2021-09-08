using System.Collections;

namespace ConsoleAppSocketServer.Domain
{
    public class User
    {
        public string Name { get; set; }
        
        public ArrayList AcquireGames { get; set; }
        
        public ArrayList PublishedGames { get; set; }
        
        public void PublishGame(Game game)
        {
            this.PublishedGames.Add(game);
        }

        public void PublishQualification(Qualification qualification)
        {
            qualification.game.AddComunityQualification(qualification);
        }

        public void BuyGame(Game game)
        {
            this.AcquireGames.Add(game);
        }

        public override bool Equals(object? obj)
        {
            User user = (User) obj;
            return this.Name.Equals(user.Name);
        }
    }
}