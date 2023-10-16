namespace TradingFutures.Shared.Services.CacheEntries.Base
{
    public abstract class BaseDictCacheEntry<TKey, TValue> : BaseCacheEntry<TValue>
       where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _dict = new();

        protected abstract TKey GetKey(TValue value);

        public int GetCount()
        {
            lock (_dict)
            {
                return _dict.Count;
            }
        }

        public TValue? GetByKey(TKey key)
        {
            if (key is null)
                return default;

            lock (_dict)
            {
                return _dict.ContainsKey(key)
                    ? _dict[key]
                    : default;
            }
        }

        public Dictionary<TKey, TValue> GetDict()
        {
            var result = new Dictionary<TKey, TValue>();

            lock (_dict)
            {
                foreach (var kvp in _dict)
                    result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }

        public List<TValue> GetList()
        {
            var result = new List<TValue>();

            lock (_dict)
            {
                foreach (var kvp in _dict)
                    result.Add(kvp.Value);
            }

            return result;
        }

        public virtual async Task ReplaceAllAsync(IEnumerable<TValue> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            lock (_dict)
            {
                _dict.Clear();

                foreach (var value in list)
                {
                    if (value != null)
                        _dict[GetKey(value)] = value;
                }
            }

            await RaiseUpdated(list);
        }

        public virtual async Task UpdateAsync(TValue value)
        {
            TKey? key = default;

            lock (_dict)
            {
                if (value != null)
                {
                    key = GetKey(value);
                    _dict[key] = value;
                }
            }

            if (value != null)
                await RaiseUpdated(new TValue[] { value });
        }

        public virtual async Task UpdateAsync(IEnumerable<TValue> list, bool clear = false)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            lock (_dict)
            {
                if (clear)
                    _dict.Clear();

                foreach (var value in list)
                {
                    if (value != null)
                        _dict[GetKey(value)] = value;
                }
            }

            await RaiseUpdated(list);
        }
    }
}