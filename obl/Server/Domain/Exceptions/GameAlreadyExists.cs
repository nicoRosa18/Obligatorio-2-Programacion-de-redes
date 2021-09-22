using System;

namespace Server.Domain.ServerExceptions
{
    public class GameAlreadyExists : Exception
    {
        public GameAlreadyExists()
        {
        }

        public override string Message => "Game already exists";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}