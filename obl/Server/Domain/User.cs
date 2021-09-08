using System.Collections;

namespace ConsoleAppSocketServer.Domain
{
    public class User
    {
        public string Name { get; set; }
        
        public ArrayList AcquireGames { get; set; }
        
        public ArrayList PublishedGames { get; set; }
    }
}