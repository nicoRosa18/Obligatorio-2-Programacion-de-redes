using System;
using System.Threading;
using System.Threading.Tasks;
using CommonLogs;
using CommonLogs.SettingsManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ServerLogs.Container;
using ServerLogs.MessageQueue.Bus;

namespace ServerLogs.MessageQueue
{
    public class LocalReceiver : BackgroundService
    {
        private readonly ILogger<LocalReceiver> _logger;
        private readonly IMessage _messageControl;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogContainer _logContainer;
        private readonly string _hostName;
        private readonly string _queueName;

        public LocalReceiver(ILogger<LocalReceiver> logger, IServiceProvider serviceProvider, ILogContainer logContainer)
        {
            _logContainer = logContainer;
            _logger = logger;
            _serviceProvider = serviceProvider;
            
            SettingsManager toRead = new SettingsManager();
            _hostName = toRead.ReadSetting("HostName");
            _queueName = toRead.ReadSetting("QueueName");
            IModel channel = new ConnectionFactory() {HostName = _hostName, DispatchConsumersAsync = true}
                                                .CreateConnection().CreateModel();
            _messageControl = new Message(channel);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageControl.ReceiveAsync<Log>(_queueName,
                    x => { Task.Run(async () => {await ReceiveItemAsync(x); }, stoppingToken); });
        }

        private async Task ReceiveItemAsync(Log logItem)
        {
            try
            {
                await _logContainer.AddLogAsync(logItem);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Exception {e.Message} -> {e.StackTrace}");
            }
        }
    }
}
