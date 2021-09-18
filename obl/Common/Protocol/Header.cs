using System;
using System.Text;

namespace Common.Protocol
{

    /*
     * Protocolo -> XXX YY ZZZZ <DATA>
     *   XXX -> REQ/RES -> Si es REQ, va del client al server, cuando el server contesta, es un RES
     *    YY -> ID COMANDO 
     *  ZZZZ -> LARGO
     *  <DATA> -> los datos en si del comando
     */ 
    
    // RES040015XXXXXXXXXXXXXXX
    
    public class Header
    {
        public string SDirection {get; set;}
        public int ICommand {get; set;}
        public int IDataLength {get; set;}

        private byte[] _direction;
        private byte[] _command;
        private byte[] _dataLength;

        public Header()
        {
        }

        public Header(string direction, int command, int datalength)
        {
            _direction = Encoding.UTF8.GetBytes(direction);
            var stringCommand = command.ToString("D2");  //Maximo largo 2, si es menor a 2 cifras, completo con 0s a la izquierda 
            _command = Encoding.UTF8.GetBytes(stringCommand);
            var stringData = datalength.ToString("D4");  // 0 < Largo <= 9999 
            _dataLength = Encoding.UTF8.GetBytes(stringData);
        }

        public byte[] GetRequest()
        {
            var header = new byte[HeaderConstants.Request.Length + HeaderConstants.CommandLength + HeaderConstants.DataLength];
            Array.Copy(_direction, 0, header, 0, HeaderConstants.Request.Length);
            Array.Copy(_command, 0, header, HeaderConstants.Request.Length, HeaderConstants.CommandLength);
            Array.Copy(_dataLength, 0, header, HeaderConstants.Request.Length + HeaderConstants.CommandLength, HeaderConstants.DataLength);
            
            return header;
        }

        public bool DecodeData(byte[] data)
        {
            try
            {
                SDirection = Encoding.UTF8.GetString(data, 0, HeaderConstants.Request.Length);
                var command = Encoding.UTF8.GetString(data, HeaderConstants.Request.Length, HeaderConstants.CommandLength);
                ICommand = int.Parse(command);
                var dataLength = Encoding.UTF8.GetString(data, HeaderConstants.Request.Length + HeaderConstants.CommandLength, HeaderConstants.DataLength);
                IDataLength = int.Parse(dataLength);
                
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

    }
}