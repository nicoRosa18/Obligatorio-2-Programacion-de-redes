using System;

namespace Common.Communicator.Exceptions
{
    public class ClientClosingException : Exception
    {
        public ClientClosingException()
        {
        }

        public override string Message => "Client is closing";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}