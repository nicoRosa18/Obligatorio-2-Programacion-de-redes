using System;
using System.Threading.Tasks;
using ServerAdmin.ServerCommunication;

namespace ServerAdmin.AdminLogic
{
    public class Logic : ILogic
    {
        private IGrpcManager _communication;

        public Logic()
        {
            _communication = new GrpcManager();
        }

        public async Task<int> TestMethodAsync()
        {
            int numero = await _communication.TestMethodAsync();
            return numero;
        }
    }
}
