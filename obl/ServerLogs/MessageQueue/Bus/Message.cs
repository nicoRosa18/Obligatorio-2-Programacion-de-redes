using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using CommonLogs;

namespace ServerLogs.MessageQueue.Bus
{
    public class Message : IMessage
    {
        private readonly IModel _channel;

        public Message(IModel channel)
        {
            _channel = channel;
        }
        
        public async Task ReceiveAsync<Log>(string queue, Action<Log> onMessage)
        {
            _channel.QueueDeclare(queue, false, false, false);
            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (s, e) =>
            {
                byte[] body = e.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Log item = JsonConvert.DeserializeObject<Log>(message); //retorna el elemento deserializado o null en el caso de un error de sintaxis
                onMessage(item);
                await Task.Yield();
            };
            _channel.BasicConsume(queue, true, consumer);
            await Task.Yield();
        }
    }
}
