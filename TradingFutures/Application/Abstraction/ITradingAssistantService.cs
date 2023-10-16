namespace TradingFutures.Application.Abstraction
{
    public interface ITradingAssistantService
    {
        Task<bool> Process();
    }
}