using System;

namespace Server.Domain.ServerExceptions
{
    public class UserNotFound : Exception
    {
        public UserNotFound()
        {
        }

        public override string Message => "User does not exist";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}