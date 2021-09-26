using System;

namespace Server.Domain.ServerExceptions
{
    public class UserNotOwnerofGame : Exception
    {
        public UserNotOwnerofGame()
        {
        }

        public override string Message => "User does not have permission to delete this game";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}