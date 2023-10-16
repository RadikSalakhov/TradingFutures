using Microsoft.EntityFrameworkCore;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Persistence.Base;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Persistence.DBModels;
using TradingFutures.Persistence.Factories;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.RepositoryServices
{
    public class SettingsItemRepositoryService : BaseRepositoryService<SettingsItemKey, SettingsItemEntity, SettingsItemDB>, ISettingsItemRepositoryService
    {
        public SettingsItemRepositoryService(DataContext dataContext)
            : base(dataContext, dataContext.GeneralSettingsItems)
        {
        }

        protected override IQueryable<SettingsItemDB> ApplyFilterByKey(IQueryable<SettingsItemDB> query, SettingsItemKey key)
        {
            return query.Where(v => v.TypeName == key.TypeName && v.PropertyName == key.PropertyName);
        }

        public async Task<IEnumerable<SettingsItemEntity>> GetByTypeName(string typeName)
        {
            var query = GetDefaultQuery();

            var records = await query.Where(v => v.TypeName == typeName).ToListAsync();

            return records.Select(v => v.ToEntity());
        }

        public async Task<ServerSettingsEntity> LoadServerSettings()
        {
            var items = await GetByTypeName(nameof(ServerSettingsEntity));

            return SettingsFactory.CreateFromItems<ServerSettingsKey, ServerSettingsEntity>(items);
        }

        public async Task<ServerSettingsEntity> SaveServerSettings(ServerSettingsEntity entity)
        {
            var sourceItems = await GetByTypeName(nameof(ServerSettingsEntity));

            var targetItems = SettingsFactory.MergeItems<ServerSettingsKey, ServerSettingsEntity>(entity, sourceItems);

            await CreateOrUpdate(targetItems);

            return await LoadServerSettings();
        }
    }
}