using Microsoft.EntityFrameworkCore;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Persistence.Base;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Persistence.DBModels;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.RepositoryServices
{
    public class TradingProfileRepositoryService : BaseRepositoryService<TradingProfileKey, TradingProfileEntity, TradingProfileDB>, ITradingProfileRepositoryService
    {
        public TradingProfileRepositoryService(DataContext dataContext)
            : base(dataContext, dataContext.TradingProfiles)
        {
        }

        protected override IQueryable<TradingProfileDB> ApplyFilterByKey(IQueryable<TradingProfileDB> query, TradingProfileKey key)
        {
            return query.Where(v => v.TradingProfileId == key.TradingProfileId);
        }

        protected override IQueryable<TradingProfileDB> GetDefaultQuery()
        {
            return DbSet.Include(v => v.TradingConditions);
        }
    }
}