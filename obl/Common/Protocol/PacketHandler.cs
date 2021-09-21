namespace Common.Protocol
{
    public static class PacketHandler
    {
        public static long GetParts(long fileSize)
        {
            var parts = fileSize / HeaderFileConstants.MaxPacketSize;
            return parts * HeaderFileConstants.MaxPacketSize == fileSize ? parts : parts + 1;
        }
    }
}