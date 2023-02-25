using HangfireApp.Interfaces;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;

namespace HangfireApp.Services
{
    public class ServiceBus : IServiceBus
    {
        private readonly IConfiguration _configuration;
        public ServiceBus(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMessageAsync(MessageContent messageContent)
        {
            IQueueClient client = new QueueClient(_configuration["ServiceBusConnectionString"], _configuration["QueueName"]);
            //Serialize car details object
            var messageBody = JsonSerializer.Serialize(messageContent);
            //Set content type and Guid
            var message = new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };
            await client.SendAsync(message);
        }
    }
}
