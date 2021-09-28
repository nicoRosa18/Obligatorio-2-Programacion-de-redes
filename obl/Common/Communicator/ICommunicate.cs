namespace Common.Communicator
{
    public interface ICommunicator
    {
        void SendMessage(int command, string message);
        CommunicatorPackage ReceiveMessage();  
        void SendFile(string message);
        string ReceiveFile();       
    }
}