namespace Common.FileManagement
{
    public interface IFileHandler
    {
        bool FileExists(string path);
        string GetFileName(string path);
        long GetFileSize(string path);
        string GetPath(string fileName);
        void DeleteFile(string path);
    }
}