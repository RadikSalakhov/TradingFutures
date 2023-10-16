using TradingFutures.Shared.Entities;

namespace TradingFutures.Application.Abstraction
{
    public interface ITradingApiClientService
    {
        Task<IEnumerable<EmaCrossEntity>> GetEmaCross(string interval);
    }
}