namespace HangfireApp.Interfaces
{
    public interface IServiceBus
    {
        Task SendMessageAsync(MessageContent message);
    }
}
