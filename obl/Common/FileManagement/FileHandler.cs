using System.IO;

namespace Common.FileManagement
{
    public class FileHandler : IFileHandler
    {
        public void DeleteFile(string path)
        {
            File.Delete(path);
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

            throw new FileNotFoundException();
        }

        public long GetFileSize(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new FileNotFoundException();
        }

        public string GetPath(string fileName)
        {
            return Path.GetFullPath(fileName);
        }
    }
}