using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Server.MessageQueue.Bus
{
    public class SendMessageQueue: ISendMessageQueue
    {
        private readonly IModel _channel;

        public SendMessageQueue(IModel channel)
        {
            _channel = channel;
        }
        
        public async Task SendAsync<Log>(string queue, Log log)
        {
            await Task.Run(() =>
            {
                _channel.QueueDeclare(queue, false, false, false);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = false;

                var output = JsonConvert.SerializeObject(log);
                _channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(output));
            });
        }
    }
}
