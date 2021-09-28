namespace Common.Communicator
{
    public class CommunicatorPackage
    {
        public string Message {get; set;}
        public int Command {get; set;}

        public CommunicatorPackage(int command, string message)
        {
            this.Message = message;
            this.Command = command;
        }
    }
}