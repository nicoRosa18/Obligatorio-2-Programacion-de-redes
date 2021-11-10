using System;
using System.Threading;
using System.Threading.Tasks;
using CommonLogs;
using CommonLogs.SettingsManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ServerLogs.MessageQueue.Bus;

namespace ServerLogs.MessageQueue
{
    public class LocalReceiver : BackgroundService
    {
        private readonly ILogger<LocalReceiver> _logger;
        private readonly IMessage _messageControl;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _HostName;
        private readonly string _QueueName;

        public LocalReceiver(ILogger<LocalReceiver> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            
            SettingsManager toRead = new SettingsManager();
            _HostName = toRead.ReadSetting("HostName");
            _QueueName = toRead.ReadSetting("QueueName");
            IModel channel = new ConnectionFactory() {HostName = _HostName}
                                                .CreateConnection().CreateModel();
            _messageControl = new Message(channel);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageControl.ReceiveAsync<Log>(_QueueName, x =>
            {
                Task.Run(() => { ReceiveItem(x); }, stoppingToken);
            });
        }

        private void ReceiveItem(Log logItem)
        {
            //sacar, solo de prueba
            _logger.LogInformation("Name: "+logItem.EventType+", Complete");
            try
            {
                
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Exception {e.Message} -> {e.StackTrace}");
            }
        }
    }
}
