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
    public class CommunicationSocket : ICommunicator
    {
        private Socket _connectedSocket;
        
        public CommunicationSocket(Socket connectedSocket){
            _connectedSocket = connectedSocket;
        }

        public CommunicatorPackage ReceiveMessage()
        {
            int headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                   HeaderConstants.DataLength;
            var buffer = new byte[headerLength];

            ReceiveData(headerLength, buffer);

            Header header = new Header();
            header.DecodeData(buffer);            
            var bufferData = new byte[header.IDataLength];  
            ReceiveData(header.IDataLength, bufferData);
            
            CommunicatorPackage toReturn = new CommunicatorPackage(header.ICommand, Encoding.UTF8.GetString(bufferData));
            return toReturn;
        }

        public void SendMessage(int command, string message)
        {        
            var header = new Header(HeaderConstants.Request, command, message.Length);
            var data = header.GetRequest();

            var sentBytes = 0;
            while (sentBytes < data.Length)
            {
                sentBytes += this._connectedSocket.Send(data, sentBytes, data.Length - sentBytes, SocketFlags.None);
            }

            sentBytes = 0;
            var bytesMessage = Encoding.UTF8.GetBytes(message);
            while (sentBytes < bytesMessage.Length)
            {
                sentBytes += this._connectedSocket.Send(bytesMessage, sentBytes, bytesMessage.Length - sentBytes,
                    SocketFlags.None);
            }
        }

        private void ReceiveData(int length, byte[] buffer)
        {
            var iRecv = 0;
            while (iRecv < length)
            {
                try
                {
                    var localRecv = this._connectedSocket.Receive(buffer, iRecv, length - iRecv, SocketFlags.None);
                    if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                    {
                        throw new ClientClosingException();
                    }

                    iRecv += localRecv;
                }
                catch (SocketException se)
                {
                    throw new ClientClosingException();
                }
            }
        }
    }
}