using System;
using System.Text;

namespace Common.Protocol
{
    public class Header
    {
        public static int GetLength()
        {
            return Specification.FixedFileNameLength + Specification.FixedFileSizeLength;
        }

        public byte[] Create(string fileName, long fileSize)
        {
            var header = new byte[GetLength()];
            var fileNameData = BitConverter.GetBytes(Encoding.UTF8.GetBytes(fileName).Length);
            if (fileNameData.Length != Specification.FixedFileNameLength)
                throw new Exception("There is something wrong with the file name");
            var fileSizeData = BitConverter.GetBytes(fileSize);

            // fileNameData -> contiene el largo del nombre del archivo
            // fileSizeData -> contiene el tamaño del archivo
            
            Array.Copy(fileNameData, 0, header, 0, Specification.FixedFileNameLength);
            Array.Copy(fileSizeData, 0, header, Specification.FixedFileNameLength, Specification.FixedFileSizeLength);

            return header;
        }
    }
}