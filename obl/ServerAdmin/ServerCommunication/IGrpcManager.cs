using System;
using System.Threading.Tasks;

namespace ServerAdmin.ServerCommunication
{
    public interface IGrpcManager
    {
        Task<int> TestMethodAsync();
    }
}
