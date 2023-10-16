namespace TradingFutures.Application.Abstraction
{
    public interface ITelegramService
    {
        Task SendMessage(string message);

        Task EnqueueMessage(string message);

        Task ProcessMessages();
    }
}