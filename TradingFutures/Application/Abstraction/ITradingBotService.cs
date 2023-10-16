namespace TradingFutures.Application.Abstraction
{
    public interface ITradingBotService
    {
        Task<bool> Process();
    }
}