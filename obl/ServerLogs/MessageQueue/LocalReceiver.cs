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
            _logger = logger;
            _serviceProvider = serviceProvider;
            
            SettingsManager toRead = new SettingsManager();
            _hostName = toRead.ReadSetting("HostName");
            _queueName = toRead.ReadSetting("QueueName");
            IModel channel = new ConnectionFactory() {HostName = _hostName}
                                                .CreateConnection().CreateModel();
            _messageControl = new Message(channel);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageControl.ReceiveAsync<Log>(_queueName, x =>
            {
                Task.Run(() => { ReceiveItem(x); }, stoppingToken);
            });
        }

        private void ReceiveItem(Log logItem)
        {
            //sacar, solo de prueba
            _logger.LogInformation("Name: "+logItem.User+", Complete");
            try
            {
                _logContainer.AddLog(logItem);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Exception {e.Message} -> {e.StackTrace}");
            }
        }
    }
}
