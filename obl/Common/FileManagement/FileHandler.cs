using System;
using System.IO;

namespace Common.FileManagement
{
    public class FileHandler : IFileHandler
    {
        public void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch(Exception e)
            {
                throw new Exception("File does not exist");
            }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileName(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Name;
            }

            throw new Exception("File does not exist");
        }

        public long GetFileSize(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception("File does not exist");
        }

        public string GetPath(string fileName)
        {
            return Path.GetFullPath(fileName);
        }
    }
}