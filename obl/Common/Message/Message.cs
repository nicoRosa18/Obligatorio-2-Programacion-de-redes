public abstract class Message
{
    public abstract string MainMenuMessage {get ; set;}
    public abstract string StartUpMessage {get ; set;}
    public abstract string UserRegistration {get ; set;}
    public abstract string UserRepeated {get ; set;}
    public abstract string UserCreated {get ; set;}                                  
    public abstract string BackToStartUpMenu {get ; set;} //move up
    public abstract string UserLogIn {get ; set;}
    public abstract string UserIncorrect {get ; set;}
    public abstract string EmptyCatalogue {get ; set;}
    public abstract string NewGameInit {get ; set;}                                
    public abstract string GameGenre {get ; set;}
    public abstract string GameSynopsis {get ; set;}
    public abstract string GameAgeRestriction {get ; set;}
    public abstract string GameCover {get ; set;}
    public abstract string GameAdded {get ; set;}
    public abstract string CatalogueView {get ; set;}
    public abstract string InvalidOption {get ; set;}
    public abstract string BuyGame {get ; set;}
    public abstract string GamePurchased {get ; set;}
    public abstract  string SearchByTitle { get; set; }
    public abstract  string SearchByGenre { get; set; }
    public abstract  string SearchByStars { get; set; }
    public abstract string SearchGame { get;set; }
    public abstract string  SearchGameOptions { get; set; }
    public abstract string MyGamesOptions { get; set; }
    public abstract string GameDetails { get; set; }
    public abstract string  PublishQualification { get; set; }
    public abstract  string QualificationAdded { get; set; }
    public abstract  string SendGameCover { get; set; }
}