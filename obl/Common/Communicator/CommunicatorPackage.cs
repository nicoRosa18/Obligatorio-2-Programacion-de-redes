using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common.Protocol;

namespace Common.Communicator
{
    public class CommunicatorPackage
    {
        public string Message {get; set;}
        public int Command {get; set;}

        public CommunicatorPackage(int command, string message)
        {
            this.Message = message;
            this.Command = command;
        }
    }
}