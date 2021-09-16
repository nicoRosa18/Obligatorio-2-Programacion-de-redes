using System;

namespace Server.Domain.ServerExceptions
{
    public class ServerClosingException : Exception
    {
        public ServerClosingException()
        {
        }

        public override string Message => "Server is closing";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}