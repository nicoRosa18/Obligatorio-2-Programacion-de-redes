using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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