using System;

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

    public override string GameCover { get; set; }

    public override string GameAdded { get; set; }

    public override string CatalogueView { get; set; }

    public override string InvalidOption { get; set; }

    public override string BuyGame { get; set; }

    public override string GamePurchased { get; set; }

    public SpanishMessage()
    {
        Setup();
    }

    private void Setup()
    {
        StartUpMessage ="Bienvenido, envie el numero para la operacion deseada \n" +
                        " 1-Registrar Usuario \n" +
                        " 2-Ingresar Usuario \n" +
                        " exit para salir";   
                        
       MainMenuMessage ="Menu de inicio. ecriba el comando para la opcion elegida \n " +
                        "4- Ver catalogo de juegos \n" +
                        "Agregar juego- Agregar un nuevo juego a la tienda \n" +
                        "Mis juegos- Ver mis juegos adiquiridos";
       
       UserRegistration = "Registro de usuario \n \n nombre de usuario: \n";
       
       UserRepeated = "Ya existe un usuario con el mismo nombre, ingrese 1 para volver a intentar";
       
       UserCreated = "Usuario Registrado!, usuarios registrados: ";
       
       BackToStartUpMenu = "Ingrese 0 para volver al menu de inicio";
       
       UserLogIn = "Ingreso de usuario, ingrese username:";
       
       UserIncorrect = "usuario incorrecto, vuelva a intentarlo.";
       
       EmptyCatalogue = "No hay juegos para mostrar \n" +
                        "escriba menu para volver al menu de inicio";
        
       NewGameInit = "Agregar nuevo juego: \n \n " +
                     "ingrese titulo: ";
       
       GameGenre = "Ingrese genero:";
       
       GameSynopsis = "ingrese una breve sinopsis:";
       
       GameAgeRestriction = "Agrege la calificacion de edad";
       
       GameCover = "Agregue a ruta de acceso a la caratula del juego:";
       
       GameAdded = "Juego agregado correctamente! \n " +
                   "escriba menu para volver al menu de inicio";
       
       CatalogueView = "Catalogo:" +
                       "Escriba menu para volver al menu de inicio \n" +
                       "Escriba 6 para comprar un juego y agregarlo a su biblioteca. \n";
       
       BuyGame = "Comprar juego: \n " +
                 "envie el numero del juego en catalogo que desea adquirir";
       
       InvalidOption = "Opcion invalida, por favor elija una nueva opcion.";

       GamePurchased = "Juego agregado correctamente a tu biblioteca! \n" +
                       "escriba menu para volver al menu de inicio";
    }
}