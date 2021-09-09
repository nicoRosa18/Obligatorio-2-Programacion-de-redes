using System;
using System.Net.Sockets;
using System.Text;

namespace ConsoleAppSocketServer
{
    public class MessageManager
    {
        public static string EliminarEspacios(string message)
        {
            int coutAt=message.IndexOf("*");
            string newMessage = message.Substring(0, coutAt);
            return newMessage;
        }

        public static string ShowMessage(Socket socketClient)
        {
            var buffer = new byte[1024];
            var bytesReceived = 1;
            bytesReceived = socketClient.Receive(buffer);
            var messageCleared = "";
            if (bytesReceived > 0)
            {
                var message = Encoding.UTF8.GetString(buffer);
                messageCleared = EliminarEspacios(message);
            }
            return messageCleared;
        }
        
    }
}