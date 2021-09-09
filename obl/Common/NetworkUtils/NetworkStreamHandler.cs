using System.Net.Sockets;
using Common.NetworkUtils.Interfaces;

namespace Common.NetworkUtils
{
    public class NetworkStreamHandler : INetworkStreamHandler
    {
        private readonly NetworkStream _networkStream;

        public NetworkStreamHandler(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public byte[] Read(int length)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = _networkStream.Read(data, dataReceived, length - dataReceived);
                if (received == 0)
                {
                    throw new SocketException();
                }
                dataReceived += received;
            }

            return data;
        }

        public void Write(byte[] data)
        {
            _networkStream.Write(data, 0, data.Length);
        }
    }
}