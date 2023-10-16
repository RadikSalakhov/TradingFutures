using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Persistence.Base;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Persistence.DBModels;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.RepositoryServices
{
    public class TradingPositionSettingsRepositoryService : BaseRepositoryService<TradingPositionSettingsKey, TradingPositionSettingsEntity, TradingPositionSettingsDB>, ITradingPositionSettingsRepositoryService
    {
        public TradingPositionSettingsRepositoryService(DataContext dataContext)
            : base(dataContext, dataContext.TradingPositionSettings)
        {
        }

        protected override IQueryable<TradingPositionSettingsDB> ApplyFilterByKey(IQueryable<TradingPositionSettingsDB> query, TradingPositionSettingsKey key)
        {
            return query.Where(v => v.Asset == key.Asset && v.OrderSide == key.OrderSide);
        }
    }
}