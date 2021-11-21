using System;
using System.Threading.Tasks;

namespace ServerAdmin.ServerCommunication
{
    public interface IGrpcManager
    {
        Task<Reply> AddUserAsync(string userName);
    }
}
