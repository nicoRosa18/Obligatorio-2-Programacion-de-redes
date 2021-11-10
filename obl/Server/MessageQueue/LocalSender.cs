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
        private readonly string _HostName;
        private readonly string _QueueName;
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
            _HostName = rabbitConfiguration.ReadSetting("HostName");
            _QueueName = rabbitConfiguration.ReadSetting("QueueName");

            IModel channel = new ConnectionFactory() {HostName = _HostName}
                                                .CreateConnection().CreateModel();
            _messageControl = new SendMessageQueue(channel);
        }

        public async Task ExecuteAsync(Log logItem)
        {
            _messageControl.SendAsync<Log>(_QueueName, logItem);
        }
    }
}
