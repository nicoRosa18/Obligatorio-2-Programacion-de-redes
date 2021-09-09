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
    class ServerHandler
    {
        private readonly TcpListener _tcpListener;
        private readonly IFileHandler _fileHandler;
        private readonly IFileStreamHandler _fileStreamHandler;
        private TcpClient _tcpClient;
        private INetworkStreamHandler _networkStreamHandler;

        public ServerHandler()
        {
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 6000);
            _fileHandler = new FileHandler();
            _fileStreamHandler = new FileStreamHandler();
        }

        public void StartServer()
        {
            _tcpListener.Start(1);
            _tcpClient = _tcpListener.AcceptTcpClient();
            _tcpListener.Stop();
            _networkStreamHandler = new NetworkStreamHandler(_tcpClient.GetStream());
        }

        public void SendFile(string path)
        {
            // El envio del archivo se compone de las siguientes etapas:
            // 1) Creo un paquete de datos que tiene esta estructura XXXX YYYYYYYY <NOMBRE>
            //          a) XXXX -> Largo del nombre del archivo
            //          b) YYYYYYYY -> Tamaño en bytes del archivo
            //          c) <NOMBRE> -> Nombre del archivo
            
            var fileSize = _fileHandler.GetFileSize(path); //Obtenemos el tamaño del archivo
            var fileName = _fileHandler.GetFileName(path); //Obtenemos el nombre del archivo
            var header = new Header().Create(fileName, fileSize);
            _networkStreamHandler.Write(header);

            _networkStreamHandler.Write(Encoding.UTF8.GetBytes(fileName));

            // 2) Calculo tamaño y cantidad de partes a enviar
            var parts = SpecificationHelper.GetParts(fileSize);
            Console.WriteLine("Will Send {0} parts",parts);
            long offset = 0;
            long currentPart = 1;

            // 3) Mientras tengo partes, envio
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
                    data = _fileStreamHandler.Read(path, offset, Specification.MaxPacketSize);
                    offset += Specification.MaxPacketSize;
                }

                _networkStreamHandler.Write(data);
                currentPart++;
            }
        }
    }
}