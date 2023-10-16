namespace TradingFutures.Shared.Services.CacheEntries.Base
{
    public abstract class BaseValueCacheEntry<TValue> : BaseCacheEntry<TValue>
        where TValue : class, new()
    {
        private TValue _value = new();

        public TValue Get()
        {
            return _value;
        }

        public virtual async Task UpdateAsync(TValue value)
        {
            _value = value ?? new();

            await RaiseUpdated(new TValue[] { _value });
        }
    }
}