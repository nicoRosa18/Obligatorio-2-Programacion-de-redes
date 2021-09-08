using System;
using System.Net.Sockets;
using System.Text;

namespace ConsoleAppSocketServer.Domain
{
    public class MessagesManager
    {
        private static UsersAndCatalogueManager _usersAndCatalogueManager = new UsersAndCatalogueManager();
        public static void MessageInterpreter(string message, Socket connectedSocket)
        {
            string messageReturn = "";
            switch (message)
            {
                case "startupMenu":
                     StartUpMenu(connectedSocket);
                    break;
                case "1":
                    UserRegist(connectedSocket);
                    messageReturn = " Registro de usuario \n " +
                                    " ingrese nombre";
                    break;
                case "3":
                    StartUpMenu(connectedSocket);
                    break;
                default: 
                 messageReturn= "por favor envie una opcion correcta";
                 break;
            }
        }

        private static void UserRegist(Socket connectedeSocket)
        {
            string messageToSend = "Registro de usuario \n \n nombre de usuario: \n";
            SendMessage(messageToSend,connectedeSocket);
            string userName=Receive(connectedeSocket);
            if (_usersAndCatalogueManager.ContainsUser(userName))
                messageToSend = "Ya existe un usuario con el mismo nombre, ingrese 1 para volver a intentar";
            else
                messageToSend = "Usuario Registrado! \n \n ingrese 3 para volver al menu de inicio";
            
            SendMessage(messageToSend,connectedeSocket);
        }
        
        private static string Receive(Socket connectedeSocket)
        {
            var buffer = new byte[1024];
            var bytesReceived = 1;
            bytesReceived = connectedeSocket.Receive(buffer);
            if (bytesReceived > 0)
            {
                var message = Encoding.UTF8.GetString(buffer);
                return message;
            }

            throw new Exception("empty message");
        }
    private static void StartUpMenu(Socket connectedSocket)
        {
           string messageToSend= "Bienvenido, envie el numero para la operacion deseada \n" +
                   " 1-Registrar Usuario \n" +
                   " 2- Ingresar Usuario";
           SendMessage(messageToSend, connectedSocket);
        }

    private static void SendMessage(string message,Socket connectedSocket)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        connectedSocket.Send(messageBytes);
    }

    }
}