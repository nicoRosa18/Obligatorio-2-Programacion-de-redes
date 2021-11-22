using System;

namespace ServerAdmin.Exceptions
{
    public class GameException : Exception
    {
        public GameException(string message): base ("GameException: " + message) {}
    }
}
