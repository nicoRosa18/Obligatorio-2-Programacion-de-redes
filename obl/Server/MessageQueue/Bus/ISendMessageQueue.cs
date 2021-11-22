using System;
using System.Threading.Tasks;

namespace Server.MessageQueue.Bus
{
    public interface ISendMessageQueue
    {
        Task SendAsync<Log>(string queue, Log message);
    }
}
