using System;

namespace Server.Domain.ServerExceptions
{
    public class GameAlreadyExists : Exception
    {
        public override string Message => "Game already exists";
    }
}