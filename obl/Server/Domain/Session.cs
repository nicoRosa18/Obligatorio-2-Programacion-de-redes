using System;
using System.Net.Sockets;
using System.Text;

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
                case "startupMenu":
                    StartUpMenu();
                    break;
                case "1":
                    UserRegist();
                    break;
                case "2":
                    UserLogin();
                    break;
                case "3":
                    StartUpMenu();
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

        private void mainMenu()
        {
            string messageToSend = "Menu de inicio";
            SendMessage(messageToSend);
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
                    $" ingrese 3 para volver al menu de inicio";
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
            string messageToSend = "Bienvenido, envie el numero para la operacion deseada \n" +
                                   " 1-Registrar Usuario \n" +
                                   " 2- Ingresar Usuario\n" +
                                   " enter para salir";
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