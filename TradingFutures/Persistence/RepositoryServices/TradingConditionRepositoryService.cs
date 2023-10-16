using Microsoft.EntityFrameworkCore;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Persistence.Base;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Persistence.DBModels;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.RepositoryServices
{
    public class TradingConditionRepositoryService : BaseRepositoryService<TradingConditionKey, TradingConditionEntity, TradingConditionDB>, ITradingConditionRepositoryService
    {
        public TradingConditionRepositoryService(DataContext dataContext)
            : base(dataContext, dataContext.TradingConditions)
        {
        }

        protected override IQueryable<TradingConditionDB> ApplyFilterByKey(IQueryable<TradingConditionDB> query, TradingConditionKey key)
        {
            return query.Where(v =>
                v.TradingProfileId == key.TradingProfileId &&
                v.ConditionType == key.ConditionType &&
                v.Interval == key.Interval);
        }
    }
}