using System;

namespace Common.Communicator.Exceptions
{
    public class FileDoesNotExist : Exception
    {
        public FileDoesNotExist()
        {
        }

        public override string Message => "File does not exist";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}