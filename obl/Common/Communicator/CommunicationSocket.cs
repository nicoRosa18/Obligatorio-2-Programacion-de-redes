using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common.Protocol;
using Common.Communicator.Exceptions;
using Common.FileManagement;

namespace Common.Communicator
{
    public class CommunicationSocket : ICommunicator
    {
        private readonly Socket _connectedSocket;
        private readonly IFileStreamHandler _fileStreamHandler;
        private readonly IFileHandler _fileHandler;
        
        public CommunicationSocket(Socket connectedSocket){
            _connectedSocket = connectedSocket;
            _fileHandler = new FileHandler();
            _fileStreamHandler = new FileStreamHandler();
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
            Header header = new Header(HeaderConstants.Request, command, message.Length);
            byte[] data = header.GetRequest();
            
            SendData(data); 
            SendData(Encoding.UTF8.GetBytes(message));
        }

        public string ReceiveFile()
        {
            Console.WriteLine("Me llego el archivo");//
            HeaderFile header = new HeaderFile();
            int headerLength = header.GetLength();
            var buffer = new byte[headerLength];
            ReceiveData(headerLength, buffer);

            Console.WriteLine(headerLength);

            header.DecodeData(buffer);
            int fileNameSize = header.IFileNameSize;
            long fileSize = header.IFileSize;

            Console.WriteLine("tamano");
            Console.WriteLine(fileNameSize);
            Console.WriteLine(fileSize);

            ReceiveData(fileNameSize, buffer);
            string fileName = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(fileName);//

            ReceiveParts(fileName, fileSize);

            string filePath = _fileHandler.GetPath(fileName);
            Console.WriteLine(filePath);//

            Console.WriteLine("Recibido");
            Console.WriteLine(filePath);

            return filePath;
        }

        public void SendFile(string path)
        {
            if(!_fileHandler.FileExists(path)){
                throw new Exception("file does not exist");
            }
            Console.WriteLine("Enviando el archivo");
            Console.WriteLine(path);
            
            long fileSize = _fileHandler.GetFileSize(path); 
            Console.WriteLine(fileSize);
            string fileName = _fileHandler.GetFileName(path);
            Console.WriteLine(fileName); 
            int fileNameSize = Encoding.UTF8.GetBytes(fileName).Length;
            Console.WriteLine(fileNameSize);

            HeaderFile header = new HeaderFile(fileNameSize, fileSize);
            byte[] data = header.GetSendHeader();

            SendData(data);
            SendData(Encoding.UTF8.GetBytes(fileName));

            SendParts(path, fileSize);
            Console.WriteLine("Enviado");
        }

        private void SendData(byte[] toSend)
        {
            var sentBytes = 0;
            while (sentBytes < toSend.Length)
            {
                sentBytes += this._connectedSocket.Send(toSend, sentBytes, toSend.Length - sentBytes, SocketFlags.None);
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

        private void SendParts(string path, long fileSize)
        {
            long parts = PacketHandler.GetParts(fileSize);
            Console.WriteLine($"Will Send {0} parts",parts); //sacar
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _fileStreamHandler.Read(path, offset, HeaderFileConstants.MaxPacketSize);
                    offset += HeaderFileConstants.MaxPacketSize;
                }

                SendData(data);
                currentPart++;
            }
        }

        private void ReceiveParts(string fileName, long fileSize)
        {    
            long parts = PacketHandler.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;
            Console.WriteLine($"Voy a recibir un archivo de tamaño {fileSize} en {parts} partes");

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    Console.WriteLine($"Recibi un segmento de tamaño {lastPartSize}"); //sacar
                    data = new byte[lastPartSize];
                    ReceiveData(lastPartSize, data);
                    offset += lastPartSize;
                }
                else
                {
                    Console.WriteLine($"Recibi un segmento de tamaño {HeaderFileConstants.MaxPacketSize}"); //sacar
                    data = new byte[HeaderFileConstants.MaxPacketSize];
                    ReceiveData(HeaderFileConstants.MaxPacketSize, data);
                    offset += HeaderFileConstants.MaxPacketSize;
                }
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}