public abstract class Message
{
    public abstract string ClientConnectedWithServer {get ; set;}
    public abstract string ServerClosed {get ; set;}
    public abstract string Disconnected {get ; set;}
    public abstract string WrongOption {get ; set;}
    public abstract string ServerConnectedWithClient {get ; set;}
    public abstract string MainMenuMessage {get ; set;}
    public abstract string StartUpMessage {get ; set;}
    public abstract string UserRegistration {get ; set;}
    public abstract string UserRepeated {get ; set;}
    public abstract string UserCreated {get ; set;}                                  
    public abstract string BackToStartUpMenu {get ; set;}
    public abstract string UserLogIn {get ; set;}
    public abstract string UserIncorrect {get ; set;}
    public abstract string EmptyCatalogue {get ; set;}
    public abstract string NewGameInit {get ; set;}                                
    public abstract string GameGenre {get ; set;}
    public abstract string GameSynopsis {get ; set;}
    public abstract string GameAgeRestriction {get ; set;}
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
    public abstract  string InvalidUsername { get; set; }
    public abstract  string RepeatedGame { get; set; }
    public abstract  string InvalidTitle { get; set; }
    public abstract  string InvalidGenre { get; set; }
    public abstract  string InvalidSynopsis { get; set; }
    public abstract  string InvalidAge { get; set; }
    public abstract  string FileNotFound { get; set; }
    public abstract  string UserLogged { get; set; }
    public abstract  string InvalidStars { get; set; }
    public abstract  string GamesNotFound { get; set; }
    public abstract  string GameList { get; set; }
    public abstract  string GameNotFound { get; set; }
    public abstract  string DownloadCover { get; set; }
    public abstract  string SavedPathAt { get; set; }
    public abstract  string QualificationComment { get; set; }
    public abstract  string UserNotLogged { get; set; }
    public abstract  string GameAlreadyInLibrary { get; set; }
}