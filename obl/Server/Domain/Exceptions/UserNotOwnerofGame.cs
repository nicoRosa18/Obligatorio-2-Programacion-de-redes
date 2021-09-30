using System;

namespace Server.Domain.ServerExceptions
{
    public class UserNotOwnerofGame : Exception
    {
        public override string Message => "User does not have permission of this game";
    }
}