using System;
using System.Text;

namespace Common.Protocol
{
    public class HeaderFile
    {
        public int IFileNameSize {get; set;}
        public long IFileSize {get; set;}
        private byte[] _fileNameSize;
        private byte[] _fileSize;

        public int GetLength()
        {
            return HeaderFileConstants.FixedFileNameSizeLength + HeaderFileConstants.FixedFileSizeLength;
        }

        public HeaderFile()
        {
        }

        public HeaderFile (int fileNameSize, long fileSize)
        {
            var stringFileNameSize = fileNameSize.ToString("D4");  
            _fileNameSize = Encoding.UTF8.GetBytes(stringFileNameSize);
            var stringFileSize = fileSize.ToString("D8");  
            _fileSize = Encoding.UTF8.GetBytes(stringFileSize);
        }

        public byte[] GetSendHeader()
        {
            var header = new byte[GetLength()];
            Array.Copy(_fileNameSize, 0, header, 0, HeaderFileConstants.FixedFileNameSizeLength);
            Array.Copy(_fileSize, 0, header, HeaderFileConstants.FixedFileNameSizeLength, HeaderFileConstants.FixedFileSizeLength);            
            return header;
        }

        public bool DecodeData(byte[] data)
        {
            try
            {
                string fileNameSize = Encoding.UTF8.GetString(data, 0, HeaderFileConstants.FixedFileNameSizeLength);
                IFileNameSize = int.Parse(fileNameSize);
                string fileSize = Encoding.UTF8.GetString(data, HeaderFileConstants.FixedFileNameSizeLength, HeaderFileConstants.FixedFileSizeLength);
                IFileSize = int.Parse(fileSize);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}