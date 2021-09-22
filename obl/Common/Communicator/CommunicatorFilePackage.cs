namespace Common.Communicator
{
    public class CommunicatorFilePackage
    {
        public string FileName {get; set;}
        public long Data {get; set;}

        public CommunicatorFilePackage(string fileName, long data)
        {
            this.FileName = fileName;
            this.Data = data;
        }
    }
}