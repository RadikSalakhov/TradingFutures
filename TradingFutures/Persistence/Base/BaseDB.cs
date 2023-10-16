using TradingFutures.Shared.Base;

namespace TradingFutures.Persistence.Base
{
    public abstract class BaseDB<TKey, TEntity, TDB>
        where TKey : BaseKey<TKey>, new()
        where TEntity : BaseEntity<TKey, TEntity>, new()
        where TDB : BaseDB<TKey, TEntity, TDB>
    {
        public DateTime CreateDT { get; set; }

        public DateTime UpdateDT { get; set; }

        protected abstract void FromKeyInternal(TKey key);

        protected abstract TKey ToKeyInternal();

        protected abstract void FromEntityInternal(TEntity entity);

        protected abstract TEntity ToEntityInternal();

        public void FromKey(TKey key)
        {
            if (key == null || !key.IsValid())
                throw new ArgumentException("Key is not valid", nameof(key));

            FromKeyInternal(key);
        }

        public TKey ToKey()
        {
            return ToKeyInternal();
        }

        public void FromEntity(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Key == null || !entity.Key.IsValid())
                throw new ArgumentException("Key is not valid", nameof(entity.Key));

            if (!entity.Key.Equals(ToKey()))
                throw new ArgumentException("Attempt to override Key", nameof(entity.Key));

            FromEntityInternal(entity);

            CreateDT = entity.CreateDT;
            UpdateDT = entity.UpdateDT;
        }

        public TEntity ToEntity()
        {
            var entity = ToEntityInternal();

            entity.CreateDT = CreateDT;
            entity.UpdateDT = UpdateDT;

            return entity;
        }
    }
}