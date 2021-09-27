using System;

namespace Server.Domain.ServerExceptions
{
    public class UserAlreadyLoggedIn : Exception
    {
        public UserAlreadyLoggedIn()
        {
        }

        public override string Message => "User already logged in";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}