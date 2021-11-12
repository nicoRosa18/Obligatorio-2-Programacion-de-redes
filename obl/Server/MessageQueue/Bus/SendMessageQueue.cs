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
                
                //estas dos lineas hacen que no se persista el mensaje en la cola, sacar de ser necesario
                // IBasicProperties properties = _channel.CreateBasicProperties();
                // properties.Persistent = false;

                string output = JsonConvert.SerializeObject(log);
                _channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(output));
            });
        }
    }
}
