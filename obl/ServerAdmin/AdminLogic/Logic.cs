using System;
using System.Threading.Tasks;
using ServerAdmin.Exceptions;
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

        public async Task AddUserAsync(string userName)
        {
            Reply possibleError = await _communication.AddUserAsync(userName);
            if(possibleError.Error)
            {
                throw new UserException(possibleError.ErrorDescription);
            }
        }
    }
}
