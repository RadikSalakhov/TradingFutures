namespace TradingFutures.Shared.Base
{
    public abstract class BaseEntity<TKey, TEntity>
        where TKey : BaseKey<TKey>, new()
        where TEntity : BaseEntity<TKey, TEntity>
    {
        public TKey Key { get; }

        public DateTime CreateDT { get; set; }

        public DateTime UpdateDT { get; set; }

        protected BaseEntity()
            : this(new TKey())
        {
        }

        protected BaseEntity(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Key = key;

            var utcNow = DateTime.UtcNow;
            CreateDT = utcNow;
            UpdateDT = utcNow;
        }

        protected abstract TEntity CreateInstanceInternal(TKey key);

        protected abstract void CopyFromInternal(TEntity entity);

        public void CopyFrom(TEntity entity)
        {
            CopyFromInternal(entity);
        }

        public TEntity GetCopy()
        {
            var entity = CreateInstanceInternal(Key);

            entity.CreateDT = CreateDT;
            entity.UpdateDT = UpdateDT;

            entity.CopyFrom((TEntity)this);

            return entity;
        }
    }
}