namespace Common.NetworkUtils.Interfaces
{
    public interface INetworkStreamHandler
    {
        void Write(byte[] data);
        byte[] Read(int length);
    }
}