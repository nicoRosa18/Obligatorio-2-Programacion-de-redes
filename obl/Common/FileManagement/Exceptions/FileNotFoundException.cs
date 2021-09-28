using System;

namespace Common.FileManagement.Exceptions
{
    public class FileNotFoundException : Exception
    {
        public override string Message => "File not found";
    }
}