using System;

namespace Server.Domain.ServerExceptions
{
    public class UserAlreadyLoggedIn : Exception
    {
        public override string Message => "User already logged in";
    }
}