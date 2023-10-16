using Microsoft.EntityFrameworkCore;
using TradingFutures.Application.Abstraction.RepositoryServices.Base;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Shared.Base;

namespace TradingFutures.Persistence.Base
{
    public abstract class BaseRepositoryService<TKey, TEntity, TDB> : IBaseRepositoryService<TKey, TEntity>
        where TKey : BaseKey<TKey>, new()
        where TEntity : BaseEntity<TKey, TEntity>, new()
        where TDB : BaseDB<TKey, TEntity, TDB>, new()
    {
        protected DataContext Context { get; }

        protected DbSet<TDB> DbSet { get; }

        public BaseRepositoryService(DataContext context, DbSet<TDB> dbSet)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }

        protected abstract IQueryable<TDB> ApplyFilterByKey(IQueryable<TDB> query, TKey key);

        protected virtual IQueryable<TDB> GetDefaultQuery()
        {
            return DbSet.AsQueryable();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            var query = GetDefaultQuery();
            var records = await query
                .ToListAsync();

            return records.Select(v => v.ToEntity()).ToList();
        }

        public async Task<TEntity?> GetByKey(TKey key)
        {
            if (key == null || !key.IsValid())
                throw new ArgumentException("Key is not valid", nameof(key));

            var query = GetDefaultQuery();
            query = ApplyFilterByKey(query, key);

            var record = await query.FirstOrDefaultAsync();

            return record?.ToEntity();
        }

        public async Task<IEnumerable<TEntity>> CreateOrUpdate(IEnumerable<TEntity> entities)
        {
            var resultEntities = new List<TEntity>();

            foreach (var entity in entities)
            {
                var resultEntity = await CreateOrUpdate(entity);
                if (resultEntity != null)
                    resultEntities.Add(resultEntity);
            }

            return resultEntities;
        }

        public async Task<TEntity?> CreateOrUpdate(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Key == null || !entity.Key.IsValid())
                throw new ArgumentException("Key is not valid", nameof(entity));

            var query = DbSet.AsQueryable();
            query = ApplyFilterByKey(query, entity.Key);

            var record = await query.FirstOrDefaultAsync();
            if (record == null)
            {
                record = new TDB();
                record.FromKey(entity.Key);
                record.FromEntity(entity);
                DbSet.Add(record);
            }
            else
            {
                record.FromEntity(entity);
            }

            Context.SaveChanges();

            return await GetByKey(record.ToKey());
        }

        public async Task<TEntity?> UpdateProperties(TKey key, Action<TEntity> func)
        {
            if (key == null || !key.IsValid())
                throw new ArgumentException("Key is not valid", nameof(key));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var query = GetDefaultQuery();
            query = ApplyFilterByKey(query, key);

            var record = await query.FirstOrDefaultAsync();
            if (record == null)
                return null;

            var entity = record.ToEntity();
            func(entity);

            if (entity.Key == null || !entity.Key.IsValid())
                return null;

            if (!entity.Key.Equals(key))
                return null;

            record.FromEntity(entity);

            Context.SaveChanges();

            return await GetByKey(key);
        }
    }
}