using System;
using System.Threading;
using System.Threading.Tasks;
using CommonLogs;
using CommonLogs.SettingsManager;
using RabbitMQ.Client;
using Server.MessageQueue.Bus;

namespace Server.MessageQueue
{
    public class LocalSender
    {
        private static LocalSender _instance;
        private readonly ISendMessageQueue _messageControl;
        private readonly string _hostName;
        private readonly string _queueName;
        private static readonly object padlock = new object();

        public static LocalSender Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new LocalSender();
                    }
                    return _instance;
                }
            }
        }

        public LocalSender()
        {
            SettingsManager rabbitConfiguration = new SettingsManager();
            _hostName = rabbitConfiguration.ReadSetting("HostName");
            _queueName = rabbitConfiguration.ReadSetting("QueueName");

            IModel channel = new ConnectionFactory() {HostName = _hostName}
                                                .CreateConnection().CreateModel();
            _messageControl = new SendMessageQueue(channel);
        }

        public async Task ExecuteAsync(string user, string game, string eventType, string status)
        {
            Log toSend = new Log(user, game, eventType, status);
            _messageControl.SendAsync<Log>(_queueName, toSend);
        }
    }
}
