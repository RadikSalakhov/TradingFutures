using TradingFutures.Application.Abstraction.RepositoryServices.Base;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Application.Abstraction.RepositoryServices
{
    public interface ITradingTransactionRepositoryService : IBaseRepositoryService<TradingTransactionKey, TradingTransactionEntity>
    {
        Task<IEnumerable<TradingTransactionEntity>> GetAll(string? asset, TradingOrderSide? orderSide);

        Task<IEnumerable<TradingTransactionEntity>> GetByDates(DateTime minDTIncl, DateTime maxDTExcl, int? takeAmount);

        Task<IEnumerable<TradingTransactionEntity>> GetLast(int takeAmount);

        Task<Dictionary<string, Dictionary<TradingOrderSide, TradingTransactionEntity>>> GetLastTransactions();

        Task<IEnumerable<TradingTransactionEntity>> GetLastTransactions(IEnumerable<TradingPositionSettingsKey> keys);

        Task<decimal> GetPnlSum(DateTime minDTIncl, DateTime maxDTExcl);

        Task<IEnumerable<TradingPnlEntity>> GetPnlData(DateTime minDTIncl, DateTime maxDTExcl);
    }
}