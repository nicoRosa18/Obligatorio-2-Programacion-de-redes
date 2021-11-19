using System;
using System.Threading.Tasks;

namespace ServerAdmin.AdminLogic
{
    public interface ILogic
    {
        Task<int> TestMethodAsync();
    }
}
