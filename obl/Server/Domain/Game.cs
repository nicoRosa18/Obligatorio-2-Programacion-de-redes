namespace ConsoleAppSocketServer.Domain
{
    public class Game
    {
        public string Title { get; set; }
        
        public int Cover { get; set; }  // image format 
        
        public string Genre { get; set; }
        
        public string Synopsis { get; set; }
        
        public string PublicQualification { get; set; } //this is a comment from the critics
    }
}