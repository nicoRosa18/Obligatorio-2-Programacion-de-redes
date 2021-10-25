using System.Text;
using System.Net.Sockets;
using Common.Protocol;
using Common.Communicator.Exceptions;
using Common.FileManagement;
using Common.SettingsManager;
using System.Threading.Tasks;

namespace Common.Communicator
{
    public class CommunicationTcp : ICommunicator
    {
        private readonly NetworkStream  _connectedNetworkStream;
        private readonly IFileStreamHandler _fileStreamHandler;
        private readonly IFileHandler _fileHandler;
        
        public CommunicationTcp(TcpClient connectedTcpClient){
            _connectedNetworkStream = connectedTcpClient.GetStream();
            _fileHandler = new FileHandler();
            _fileStreamHandler = new FileStreamHandler();
        }

        public async Task<CommunicatorPackage> ReceiveMessageAsync()
        {
            int headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                   HeaderConstants.DataLength;
            var buffer = new byte[headerLength];

            await ReceiveDataAsync(headerLength, buffer);

            Header header = new Header();
            header.DecodeData(buffer);            
            var bufferData = new byte[header.IDataLength];  
            await ReceiveDataAsync(header.IDataLength, bufferData);
            
            CommunicatorPackage toReturn = new CommunicatorPackage(header.ICommand, Encoding.UTF8.GetString(bufferData));
            return toReturn;
        }

        public async Task SendMessageAsync(int command, string message)
        {        
            Header header = new Header(HeaderConstants.Request, command, message.Length);
            byte[] data = header.GetRequest();
            
            await SendData(data); 
            await SendData(Encoding.UTF8.GetBytes(message));
        }

        public async Task<string> ReceiveFileAsync(string path)
        {
            HeaderFile header = new HeaderFile();
            int headerLength = header.GetLength();
            byte[] buffer = new byte[headerLength];
            await ReceiveDataAsync(headerLength, buffer);

            header.DecodeData(buffer);
            int fileNameSize = header.IFileNameSize;
            long fileSize = header.IFileSize;

            buffer = new byte[fileNameSize];
            await ReceiveDataAsync(fileNameSize, buffer);
            string fileName = Encoding.UTF8.GetString(buffer);

            string existingFile = _fileHandler.GetPath(path+fileName);
            if(_fileHandler.FileExists(existingFile))
            {
                _fileHandler.DeleteFile(existingFile);
            }
            await ReceivePartsAsync(path+fileName, fileSize);

            string filePath = _fileHandler.GetPath(path+fileName);

            return filePath;
        }

        public async Task SendFileAsync(string path)
        {            
            string fileName = _fileHandler.GetFileName(path);
            int fileNameSize = Encoding.UTF8.GetBytes(fileName).Length;
            long fileSize = _fileHandler.GetFileSize(path); 

            HeaderFile header = new HeaderFile(fileNameSize, fileSize);
            byte[] data = header.GetSendHeader();

            await SendData(data);
            await SendData(Encoding.UTF8.GetBytes(fileName));

            await SendPartsAsync(path, fileSize);
        }

        private async Task SendData(byte[] toSend)
        {
            try
            {
                await this._connectedNetworkStream.WriteAsync(toSend, 0, toSend.Length).ConfigureAwait(false);;
            }
            catch(System.IO.IOException)
            {
                throw new ClientClosingException();
            }
        }

        private async Task ReceiveDataAsync(int length, byte[] buffer)
        {
            int iRecv = 0;
            while (iRecv < length)
            {
                try
                {
                    int localRecv = await this._connectedNetworkStream.ReadAsync(buffer, iRecv, length - iRecv).ConfigureAwait(false);
                    if (localRecv == 0) 
                    {
                        throw new ClientClosingException();
                    }

                    iRecv += localRecv;
                }
                catch (System.IO.IOException)
                {
                    throw new ClientClosingException();
                }
            }
        }

        private async Task SendPartsAsync(string path, long fileSize)
        {
            long parts = PacketHandler.GetParts(fileSize);
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

                await SendData(data);
                currentPart++;
            }
        }

        private async Task ReceivePartsAsync(string fileName, long fileSize)
        {    
            long parts = PacketHandler.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = new byte[lastPartSize];
                    await ReceiveDataAsync(lastPartSize, data);
                    offset += lastPartSize;
                }
                else
                {
                    data = new byte[HeaderFileConstants.MaxPacketSize];
                    await ReceiveDataAsync(HeaderFileConstants.MaxPacketSize, data);
                    offset += HeaderFileConstants.MaxPacketSize;
                }
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}