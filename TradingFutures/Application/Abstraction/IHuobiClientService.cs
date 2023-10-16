using TradingFutures.Shared.Entities;

namespace TradingFutures.Application.Abstraction
{
    public interface IHuobiClientService
    {
        event Func<TradingTransactionEntity, Task>? TransactionCompleted;

        Task<DateTime> GetServerTime();

        Task<IEnumerable<PriceTickerEntity>> GetPriceTickers();

        Task<IEnumerable<TradingPositionEntity>> GetTradingPositions();

        Task RefreshContractsInfo(string? asset = null);

        Task<bool> OpenLong(string asset, string label);

        Task<bool> CloseLong(string asset, string label);

        Task<bool> OpenShort(string asset, string label);

        Task<bool> CloseShort(string asset, string label);
    }
}