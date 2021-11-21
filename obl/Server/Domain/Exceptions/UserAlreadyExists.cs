using System;

namespace Server.Domain.ServerExceptions
{
    public class UserAlreadyExists : Exception
    {
        public override string Message => "User already exists";
    }
}