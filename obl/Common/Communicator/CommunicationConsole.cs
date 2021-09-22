using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common.Protocol;
using Common.Communicator.Exceptions;

namespace Common.Communicator
{
    public class CommunicationConsole : ICommunicator
    {
        public string ReceiveFile()
        {
            throw new NotImplementedException();
        }

        public CommunicatorPackage ReceiveMessage()
        {
            throw new NotImplementedException();
        }

        public void SendFile(string message)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(int command, string message)
        {
            throw new NotImplementedException();
        }
    }
}