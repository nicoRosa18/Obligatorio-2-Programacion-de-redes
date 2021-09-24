using System;

namespace Common.FileManagement.Exceptions
{
    public class FileNotFoundException : Exception
    {
        public FileNotFoundException()
        {
        }

        public override string Message => "File not found";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}