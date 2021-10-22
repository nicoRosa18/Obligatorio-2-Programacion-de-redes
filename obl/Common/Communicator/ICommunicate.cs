using System.Threading.Tasks;

namespace Common.Communicator
{
    public interface ICommunicator
    {
        Task SendMessageAsync(int command, string message);
        Task<CommunicatorPackage> ReceiveMessageAsync();  
        Task SendFileAsync(string message);
        Task<string> ReceiveFileAsync(string path);       
    }
}