using System;

namespace Server.Domain.ServerExceptions
{
    public class GameNotFound : Exception
    {
        public GameNotFound()
        {
        }

        public override string Message => "Game not found";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}