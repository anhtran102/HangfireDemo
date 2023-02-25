using Hangfire;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace HangfireApp
{
    public class MessageHandler: BackgroundService
    {        
        private IQueueClient _orderQueueClient;
        public int Order = 0;
        public string InstanceName = "Hangfire#5";
        Random random = new Random();        

        private readonly string QueueConnectionString = "Endpoint=sb://anhtran.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=jJVnw2gDl1meFeUSHiV1/cydR63X0mkaP+ASbMQL9Gc=";
        private readonly string QueueName = "testing";

        public MessageHandler()
        {            
        }     

        public async Task Handle(Message message, CancellationToken cancelToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var body = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"{InstanceName}: Received message at {DateTime.Now.ToLongDateString()}. body {body}");
            //Hangfire job
            BackgroundJob.Enqueue(() => BussinessLogic(body));

            await _orderQueueClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
        }

        public void BussinessLogic(string message)
        {
            Console.WriteLine($"{InstanceName}: handle message at {DateTime.Now.ToLongDateString()}: . body: {message}");
        }

        public virtual Task HandleFailureMessage(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            if (exceptionReceivedEventArgs == null)
                throw new ArgumentNullException(nameof(exceptionReceivedEventArgs));
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var messageHandlerOptions = new MessageHandlerOptions(HandleFailureMessage)
            {
                MaxConcurrentCalls = 5,
                AutoComplete = false,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(10)
            };
            _orderQueueClient = new QueueClient(QueueConnectionString, QueueName);
            _orderQueueClient.RegisterMessageHandler(Handle, messageHandlerOptions);
            Console.WriteLine($"{nameof(MessageHandler)} service has started.");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{nameof(MessageHandler)} service has stopped.");
            await _orderQueueClient.CloseAsync().ConfigureAwait(false);
        }

    }
}