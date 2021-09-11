using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using Common.Protocol;

namespace ConsoleAppSocketServer.Domain
{
    public class Session
    {
        public int ThreadId { get; set; }
        private User UserLogged { get; set; }
        public static UsersAndCatalogueManager _usersAndCatalogueManager { get; set; }

        private Socket ConnectedSocket { get; set; }
        public bool Active { get; set; }

        public Session(Socket connectedSocket, int threadId)
        {
            this.ThreadId = threadId;
            this.ConnectedSocket = connectedSocket;
            this.Active = true;
        }

        public void MessageInterpreter(string message)
        {
            string messageReturn = "";
            switch (message)
            {
                case CommandConstants.StartupMenu:
                    StartUpMenu();
                    break;
                case CommandConstants.RegisterUser:
                    UserRegist();
                    break;
                case CommandConstants.LoginUser:
                    UserLogin();
                    break;
                case CommandConstants.ViewCatalogue:
                    ShowCatalogue();
                    break;
                case CommandConstants.MainMenu:
                    mainMenu();
                    break;
                case CommandConstants.AddGame:
                    AddGame();
                    break;
                case "":
                    CloseConnection();
                    break;
                default:
                    messageReturn = "por favor envie una opcion correcta";
                    SendMessage(messageReturn);
                    break;
            }
        }

        private void AddGame()
        {
            if (UserLogged != null)
            {
                try
                {
                    string messageToSend = "Agregar nuevo juego: \n \n " +
                                           "ingrese titulo: ";
                    SendMessage(messageToSend);
                    string title = Receive();
                    messageToSend = " ingrese genero:";
                    SendMessage(messageToSend);
                    string genre = Receive();
                    messageToSend = "ingrese una breve sinopsis:";
                    SendMessage(messageToSend);
                    string synopsis = Receive();
                    messageToSend = "Agrege la calificacion de edad";
                    SendMessage(messageToSend);
                    string ageRating = Receive();
                    messageToSend = "Agregue a ruta de acceso a la caratula del juego:";
                    SendMessage(messageToSend);
                    string coverPath = Receive();
                    Game gameToAdd = new Game(title, coverPath, genre, synopsis, ageRating);
                    _usersAndCatalogueManager.AddGame(gameToAdd);
                    SendMessage(SystemMessages.GameAdded);
                }
                catch (Exception e)
                {
                    SendMessage(e.Message);
                }
            }
            else
            {
                SendMessage((SystemMessages.InvalidOption));
            }
        }

        private void UserLogin()
        {
            string messageToSend = "Ingreso de usuario, ingrese username:";
            SendMessage(messageToSend);
            string user = Receive();
            if (_usersAndCatalogueManager.Login(user))
            {
                UserLogged = _usersAndCatalogueManager.GetUser(user);
                mainMenu();
            }
            else
            {
                messageToSend = "usuario incorrecto, vuelva a intentarlo.";
                SendMessage(messageToSend);
            }
        }

        private void ShowCatalogue()
        {
            if (UserLogged != null)
            {
                Catalogue catalogue = _usersAndCatalogueManager.GetCatalogue();
                SendMessage(SystemMessages.CatalogueView+catalogue.ShowGamesOnStringList());
               // serverHandler.SendFile(SystemMessages.CatalogueView+catalogue.ShowGamesOnStringList());
            }
            else
            {
                SendMessage((SystemMessages.InvalidOption));
            }
        }

        private void mainMenu()
        {
            if (UserLogged != null)
            {
                string messageToSend = SystemMessages.MainMenuMessage;
                SendMessage(messageToSend);
            }
            else
            {
                SendMessage((SystemMessages.InvalidOption));
            }
        }

        private void UserRegist()
        {
            string messageToSend = "Registro de usuario \n \n nombre de usuario: \n";
            SendMessage(messageToSend);
            string userName = Receive();
            if (_usersAndCatalogueManager.ContainsUser(userName))
                messageToSend = "Ya existe un usuario con el mismo nombre, ingrese 1 para volver a intentar";
            else
            {
                _usersAndCatalogueManager.AddUser(userName);
                messageToSend =
                    $"Usuario Registrado!, usuarios registrados: {_usersAndCatalogueManager.Users.Count} \n \n" +
                    $" ingrese 0 para volver al menu de inicio";
            }

            SendMessage(messageToSend);
        }

        private string Receive()
        {
            var buffer = new byte[1024];
            var bytesReceived = 1;
            bytesReceived = this.ConnectedSocket.Receive(buffer);
            if (bytesReceived > 0)
            {
                var message = Encoding.UTF8.GetString(buffer);
                return message;
            }

            throw new Exception("empty message");
        }

        private void StartUpMenu()
        {
            string messageToSend = SystemMessages.StartUpMessage;
            SendMessage(messageToSend);
        }

        private void SendMessage(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message+"*");
            this.ConnectedSocket.Send(messageBytes);
        }

        private void CloseConnection()
        {
            this.Active = false;
        }

        public void Listen()
        {
            Console.WriteLine("en Listen de Session");
            // Este while se usa para mantenerse aqui mientras la conexion no se cierra
            var buffer = new byte[1024];
                // Si la conexion se cierra, el receive retorna 0
             var   bytesReceived = ConnectedSocket.Receive(buffer);
                if (bytesReceived > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer);
                    var messagecleared = MessageManager.EliminarEspacios(message);
                    Console.WriteLine(messagecleared);
                    this.MessageInterpreter(messagecleared); //interpreta el mensaje recibido y genera una respuesta
                }
                else
                {
                    Console.WriteLine($"{this.ThreadId}: El cliente remoto cerro la conexion...");
                }
        }
    }
}