using TradingFutures.Shared.Base;

namespace TradingFutures.Application.Abstraction.RepositoryServices.Base
{
    public interface IBaseRepositoryService<TKey, TEntity>
        where TKey : BaseKey<TKey>, new()
        where TEntity : BaseEntity<TKey, TEntity>, new()
    {
        Task<IEnumerable<TEntity>> GetAll();

        Task<TEntity?> GetByKey(TKey key);

        Task<IEnumerable<TEntity>> CreateOrUpdate(IEnumerable<TEntity> entities);

        Task<TEntity?> CreateOrUpdate(TEntity entity);

        Task<TEntity?> UpdateProperties(TKey key, Action<TEntity> func);
    }
}