using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.FileHandler;
using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace ConsoleArchiveSender
{
    class ClientHandler
    {
        
        private readonly TcpClient _tcpClient;
        private readonly IFileStreamHandler _fileStreamHandler;
        private INetworkStreamHandler _networkStreamHandler;

        public ClientHandler()
        {
            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6001));
            _fileStreamHandler = new FileStreamHandler();
        }

        public void StartClient()
        {
            _tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 6000);
            _networkStreamHandler = new NetworkStreamHandler(_tcpClient.GetStream());
        }

        public void ReceiveFile()
        {
            // 1) Recibo 12 bytes
            // 2) Tomo los 4 primeros bytes para saber el largo del nombre del archivo
            // 3) Tomo los siguientes 8 bytes para saber el tamaño del archivo
            var header = _networkStreamHandler.Read(Header.GetLength());
            var fileNameSize = BitConverter.ToInt32(header, 0);
            var fileSize = BitConverter.ToInt64(header, Specification.FixedFileNameLength);

            // 4) Recibo el nombre del archivo
            var fileName = Encoding.UTF8.GetString(_networkStreamHandler.Read(fileNameSize));

            // 5) Calculo la cantidad de partes a recibir
            long parts = SpecificationHelper.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            Console.WriteLine($"Voy a recibir un archivo de tamaño {fileSize} en {parts} partes");
            
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    Console.WriteLine($"Recibi un segmento de tamaño {lastPartSize}");
                    data = _networkStreamHandler.Read(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    Console.WriteLine($"Recibi un segmento de tamaño {Specification.MaxPacketSize}");
                    data = _networkStreamHandler.Read(Specification.MaxPacketSize);
                    offset += Specification.MaxPacketSize;
                }
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}