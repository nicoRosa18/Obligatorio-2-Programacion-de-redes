using System;

namespace Server.Domain.ServerExceptions
{
    public class UserNotFound : Exception
    {
        public override string Message => "User does not exist";
    }
}