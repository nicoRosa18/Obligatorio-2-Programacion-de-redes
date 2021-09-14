public class SpanishMessage : Message
{
    public override string MainMenuMessage { 
        get{return MainMenuMessage;}  
        set  
        {  
            MainMenuMessage ="Menu de inicio. ecriba el comando para la opcion elegida \n\n " +
                                    "catalogo- Ver catalogo de juegos\n" +
                                    "Agregar juego- Agregar un nuevo juego a la tienda \n" +
                                    "Mis juegos- Ver mis juegos adiquiridos";
        }
    } 
    
    public override string StartUpMessage { 
        get{return StartUpMessage;}  
        set  
        {  
            StartUpMessage ="Bienvenido, envie el numero para la operacion deseada \n" +
                                        " 1-Registrar Usuario \n" +
                                        " 2- Ingresar Usuario\n" +
                                        " Enter para salir";
        }
    } 

    public override string UserRegistration { 
        get{return UserRegistration;}  
        set  
        {  
            UserRegistration = "Registro de usuario \n \n nombre de usuario: \n";
        }
    } 

    public override string UserRepeated { 
        get{return UserRepeated;}  
        set  
        {  
            UserRepeated = "Ya existe un usuario con el mismo nombre, ingrese 1 para volver a intentar";
        }
    } 

    public override string UserCreated { 
        get{return UserCreated;}  
        set  
        {  
            UserCreated = "Usuario Registrado!, usuarios registrados: ";
        }
    } 

    public override string BackToStartUpMenu { 
        get{return BackToStartUpMenu;}  
        set  
        {  
            BackToStartUpMenu = "Ingrese 0 para volver al menu de inicio"; //move up
        }
    } 
    public override string UserLogIn { 
        get{return UserLogIn;}  
        set  
        {  
            UserLogIn = "Ingreso de usuario, ingrese username:";
        }
    } 

    public override string UserIncorrect { 
        get{return UserLogIn;}  
        set  
        {  
            UserIncorrect = "usuario incorrecto, vuelva a intentarlo.";
        }
    }

    public override string EmptyCatalogue { 
        get{return EmptyCatalogue;}  
        set  
        {  
            EmptyCatalogue = "No hay juegos para mostrar \n" +
                                          "escriba menu para volver al menu de inicio";
        }
    } 

    public override string NewGameInit { 
        get{return NewGameInit;}  
        set  
        {  
            NewGameInit = "Agregar nuevo juego: \n \n " +
                                           "ingrese titulo: ";
        }
    } 

    public override string GameGenre { 
        get{return GameGenre;}  
        set  
        {  
            GameGenre = "Ingrese genero:";
        }
    } 

    public override string GameSynopsis { 
        get{return GameSynopsis;}  
        set  
        {  
            GameSynopsis = "ingrese una breve sinopsis:";
        }
    } 

    public override string GameAgeRestriction { 
        get{return GameAgeRestriction;}  
        set  
        {  
            GameAgeRestriction = "Agrege la calificacion de edad";
        }
    } 

    public override string GameCover { 
        get{return GameCover;}  
        set  
        {  
            GameCover = "Agregue a ruta de acceso a la caratula del juego:";
        }
    } 

    public override string GameAdded { 
        get{return GameAdded;}  
        set  
        {  
            GameAdded = "Juego agregado correctamente! \n " +
                                     "escriba menu para volver al menu de inicio";
        }
    } 

    public override string CatalogueView { 
        get{return CatalogueView;}  
        set  
        {  
            CatalogueView = "Catalogo:" +
                                         "Escriba menu para volver al menu de inicio \n" +
                                         "Escriba COMPRAR-nombredeljuego para comprar el juego y agregarlo a su biblioteca. \n";
        }
    } 

    public override string InvalidOption { 
        get{return InvalidOption;}  
        set  
        {  
            InvalidOption = "Opcion invalida, por favor elija una nueva opcion.";
        }
    } 
}
