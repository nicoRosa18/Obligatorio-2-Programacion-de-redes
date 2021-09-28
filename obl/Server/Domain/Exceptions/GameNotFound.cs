using System;

namespace Server.Domain.ServerExceptions
{
    public class GameNotFound : Exception
    {
        public override string Message => "Game not found";
    }
}