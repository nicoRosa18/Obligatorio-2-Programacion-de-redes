namespace Server.Domain
{
    public class Qualification
    {
        public string User { get; set; }
        
        public Game Game { get; set; }

        public int Stars { get; set; }
        
        public string Comment { get; set; }

        public Qualification(){}
    }
}