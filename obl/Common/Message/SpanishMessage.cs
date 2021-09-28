using Common.Protocol;

public class SpanishMessage : Message
{
    public override string MainMenuMessage { get;set;} 
    public override string StartUpMessage { get; set; }
    public override string UserRegistration { get; set; } 
    public override string UserRepeated { get; set; } 
    public override string UserCreated { get; set; } 
    public override string BackToStartUpMenu { get; set; } 
    public override string UserLogIn { get; set; } 
    public override string UserIncorrect { get; set; }
    public override string EmptyCatalogue { get; set; } 
    public override string NewGameInit { get; set; } 
    public override string GameGenre { get; set; }
    public override string GameSynopsis { get; set; }
    public override string GameAgeRestriction { get; set; }
    public override string GameAdded { get; set; }
    public override string CatalogueView { get; set; }
    public override string InvalidOption { get; set; }
    public override string BuyGame { get; set; }
    public override string GamePurchased { get; set; }
    public override  string SearchByTitle { get; set; }
    public override  string SearchByGenre { get; set; }
    public override  string SearchByStars { get; set; }
    public override string SearchGame { get; set; }
    public override string SearchGameOptions { get; set; }
    public override string MyGamesOptions { get; set; }
    public override string GameDetails { get; set; }
    public override string PublishQualification { get; set; }
    public override string QualificationAdded { get; set; }
    public override string SendGameCover { get; set; }
    public override string ClientConnectedWithServer { get; set; }
    public override string ServerConnectedWithClient { get; set; }
    public override string ServerClosed { get; set; }
    public override string WrongOption { get; set; }
    public override string Disconnected { get; set; }
    public override string InvalidUsername { get; set; }
    public override string RepeatedGame { get; set; }
    public override string InvalidTitle { get; set; }
    public override string InvalidGenre { get; set; }
    public override string InvalidSynopsis { get; set; }
    public override string InvalidAge { get; set; }
    public override string FileNotFound { get; set; }
    public override string UserLogged { get; set; }
    public override string InvalidStars { get; set; }
    public override string GamesNotFound { get; set; }
    public override string GameList { get; set; }
    public override string GameNotFound { get; set; }
    public override string DownloadCover { get; set; }
    public override string SavedPathAt { get; set; }
    public override string QualificationComment { get; set; }
    public override string UserNotLogged { get; set; }
    public override string GameAlreadyInLibrary { get; set; }
    public override string GameModifiedCorrectly { get; set; }
    public override string GameRemovedCorrectly { get; set; }
    public override string UserNotGameOwner { get; set; }
    public override string ChangeMenu { get; set; }
    public override string GameDeleted { get; set; }
    public override string GameModified { get; set; }
    public override string GameModifiedNewInfo { get; set; }
    public override string UserAlreadyLoggedIn { get; set; }
    public override string NoParametersToSearch { get; set; }
    public override string ErrorGameCover { get; set; }

    public SpanishMessage()
    {
        Setup();
    }

    private void Setup()
    {
        ClientConnectedWithServer = "Conectado al server remoto";

        ServerClosed = "Server cerrado, desconectando...";

        Disconnected = "Desconectado";

        WrongOption = "Por favor envie una opcion correcta";

        StartUpMessage ="Sistema Redes \n" +
                        "Envie el numero de la operacion deseada \n" +
                        " 1-Registrar Usuario \n" +
                        " 2-Ingresar Usuario \n" +
                        " Inserte exit para salir";

        MainMenuMessage = "Menu del Sistema \n" +
                            "Escriba el comando para la opcion elegida \n " +
                            $" {CommandConstants.SearchGame}-Buscar juego \n "+
                            $" {CommandConstants.AddGame}-Agregar juego (Agrega un nuevo juego a la tienda) \n " +
                            $" {CommandConstants.MyGames}-Ver mis juegos adiquiridos";

        ChangeMenu = $"{CommandConstants.MainMenu}-Para vover al menu \n" +
                     $"{CommandConstants.buyGame}- Para comprar un juego \n \n"+
                    "Estas opciones solo estan permitidas si es el publicador del juego: \n" +
                        $" {CommandConstants.RemoveGame}-Para remover el juego \n"+
                        $" {CommandConstants.ModifyGame}-Para modificar el juego \n";

        GameDeleted = "Eliminacion de juego \n" +
                        "Ingrese el nombre del juego a borrar:";

        GameModified = "Mofificacion de juego \n" +
                        "Ingrese el nombre del juego a modificar:";

        GameModifiedNewInfo = "Ingrese la nueva informacion (Si se deja en blanco se asumen los valores anteriores)";
       
        UserRegistration = "Registro de usuario \n" +
                            "nombre de usuario: ";
       
        UserRepeated = "Ya existe un usuario con el mismo nombre, ingrese 1 para volver a intentar";
       
        UserCreated = "Usuario Registrado!, usuarios registrados: ";
       
        BackToStartUpMenu = "Ingrese 0 para volver al menu de inicio";
       
        UserLogIn = "Ingreso de usuario \n " +
                        "ingrese nombre de usuario:";

        UserLogged = "Usuario loggeado";
        
        UserIncorrect = "Usuario incorrecto, vuelva a intentarlo.\n" +
                        $"{CommandConstants.StartupMenu} para volver al menu de inico";

        UserNotLogged = "No es posible realizar la accion porque no se loggeo el usuario";

        EmptyCatalogue = "No hay juegos para mostrar \n";

        NewGameInit = "Agregar nuevo juego: \n \n" + 
                        "Ingrese titulo: ";
        
        GameGenre = "Ingrese genero:";
        
        GameSynopsis = "Ingrese una breve sinopsis:";
        
        GameAgeRestriction = "Agrege la calificacion de edad";
        
        GameAdded = "Juego agregado correctamente! \n" +
                    "Recuerda que los juegos recien agregados cuentan con 0 estrellas \n";
        
        CatalogueView = "Catalogo: \n ";
        
        BuyGame = "Comprar juego: \n " +
                    "Envie el nombre del juego buscado anteriormente que desea adquirir";
        
        InvalidOption = "Opcion invalida, por favor elija una nueva opcion.";

        InvalidUsername = "Nombre no valido";

        GamePurchased = "Juego agregado correctamente a tu biblioteca! \n";

        SearchGame = "Buscar juego: si no desea buscar por un atributo, dejelo vacio";

        SearchByTitle = "Titulo:";

        SearchByGenre = "Genero:";

        SearchByStars = "Calificacion (0-5):";

        SearchGameOptions = "Opciones: " +
                            $" {CommandConstants.MainMenu}-Para volver al menu \n" +
                            $" {CommandConstants.buyGame}-Para comprar un juego \n"+
                            $" {CommandConstants.GameDetails}-Para ver los detalles de un juego \n"+
                            $" {CommandConstants.PublishQualification}-Para agregar una calificacion";

        MyGamesOptions = $"{CommandConstants.MainMenu}-Para vover al menu \n" +
                        $"{CommandConstants.GameDetails}-Para ver los detalles de un juego \n"+
                        $"{CommandConstants.PublishQualification} Para agregar una calificacion";

        GameDetails = $"Ingrese el nombre del juego que quiere ver los detalles";

        PublishQualification = "Ingrese el nombre del juego que quiere agregar una calificacion";

        QualificationComment = "Agregue un comentario: ";

        QualificationAdded = "Calificacion agregada!";

        SendGameCover = "Ingrese la ruta de la imagen";

        RepeatedGame = "Este juego ya esta ingresado en el sistema, pruebe con otro titulo";

        InvalidTitle = "Titulo no valido";

        InvalidGenre = "Genero no valido";

        InvalidSynopsis = "Sinopsis no valida";

        InvalidAge = "Calificacion no valida";

        FileNotFound = "Archivo no encontrado, reingrese su ruta";

        InvalidStars = "Debe ingresar un numero entre 0 y 5";

        GamesNotFound = "No se encontaron juegos";

        GameNotFound = "No se encontro el juego";

        GameList = "Lista de Juegos: ";

        DownloadCover = "Desea descargar la caratula?" +
                            " 1-Si \n" +
                            " Cualquier otro input para seguir \n";
        
        SavedPathAt = "Archivo guardado en la siguiente direccion:";

        GameAlreadyInLibrary = "El juego ya esta en tu libreria";
        
        GameModifiedCorrectly = "Juego modificado correctamente!";

        GameRemovedCorrectly = "Juego eliminado correctamente!";

        UserNotGameOwner = "Tu usuario no es el dueno del juego, no cuenta con permisos";

        UserAlreadyLoggedIn = $"Este usuario ya tiene una sesion iniciada \n" +
                              $"{CommandConstants.StartupMenu} para volver al menu de inico ";
        
        NoParametersToSearch = "No se especifican parametros para la busqueda, volviendo al menu principal.";

        ErrorGameCover = "No se encuentra el archivo en la ruta especificado, por favor igresela de nuevo";
    }
}