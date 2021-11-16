using System;
using System.Threading.Tasks;

namespace ServerLogs.MessageQueue.Bus
{
    public interface IMessage
    {
        Task ReceiveAsync<Log>(string queue, Action<Log> onMessage);
    }
}
