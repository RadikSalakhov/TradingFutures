namespace TradingFutures.Shared.Services.CacheEntries.Base
{
    public abstract class BaseDoubleDictCacheEntry<TKeyA, TKeyB, TValue> : BaseCacheEntry<TValue>
        where TKeyA : notnull
        where TKeyB : notnull
    {
        private readonly Dictionary<TKeyA, Dictionary<TKeyB, TValue>> _dict = new();

        protected abstract TKeyA GetKeyA(TValue value);

        protected abstract TKeyB GetKeyB(TValue value);

        //protected virtual void OnElementUpdated(TValue prevValue, TValue curValue)
        //{
        //}

        public TValue? GetByKeys(TKeyA keyA, TKeyB keyB)
        {
            lock (_dict)
            {
                if (!_dict.TryGetValue(keyA, out Dictionary<TKeyB, TValue>? dictB))
                    return default;

                return dictB.ContainsKey(keyB)
                    ? dictB[keyB]
                    : default;
            }
        }

        public IEnumerable<TValue> GetValues()
        {
            var result = new List<TValue>();

            lock (_dict)
            {
                foreach (var kvpA in _dict)
                    foreach (var kvpB in kvpA.Value)
                        result.Add(kvpB.Value);
            }

            return result;
        }

        public IEnumerable<TValue> GetValues(IEnumerable<TKeyA> keysA)
        {
            if (keysA == null || !keysA.Any())
                return Array.Empty<TValue>();

            var result = new List<TValue>();

            lock (_dict)
            {
                foreach (var kvpA in _dict)
                {
                    if (!keysA.Contains(kvpA.Key))
                        continue;

                    foreach (var kvpB in kvpA.Value)
                        result.Add(kvpB.Value);
                }
            }

            return result;
        }

        public Dictionary<TKeyA, Dictionary<TKeyB, TValue>> GetFirstDict()
        {
            var result = new Dictionary<TKeyA, Dictionary<TKeyB, TValue>>();

            lock (_dict)
            {
                foreach (var kvpFirst in _dict)
                {
                    var secondDict = new Dictionary<TKeyB, TValue>(kvpFirst.Value);
                    result.Add(kvpFirst.Key, secondDict);
                }
            }

            return result;
        }

        public Dictionary<TKeyB, Dictionary<TKeyA, TValue>> GetFirstDictReversed()
        {
            var resultDict = new Dictionary<TKeyB, Dictionary<TKeyA, TValue>>();

            lock (_dict)
            {
                foreach (var kvpFirst in _dict)
                {
                    foreach (var kvpSecond in kvpFirst.Value)
                    {
                        if (!resultDict.TryGetValue(kvpSecond.Key, out Dictionary<TKeyA, TValue>? secondDict))
                        {
                            secondDict = new Dictionary<TKeyA, TValue>();
                            resultDict.Add(kvpSecond.Key, secondDict);
                        }

                        secondDict[kvpFirst.Key] = kvpSecond.Value;
                    }
                }
            }

            return resultDict;
        }

        public Dictionary<TKeyB, TValue> GetSecondDict(TKeyA keyA)
        {
            lock (_dict)
            {
                if (!_dict.TryGetValue(keyA, out Dictionary<TKeyB, TValue>? secondDict))
                    return new();

                return new Dictionary<TKeyB, TValue>(secondDict);
            }
        }

        //public IEnumerable<IGrouping<TKeyA, IEnumerable<TValue>>> GetGroupByKeyA()
        //{
        //    lock (_dict)
        //    {
        //        return _dict.GroupBy(g => g.Key, g => g.Value.SelectMany(v=>v.Value))
        //    }
        //}

        //public Dictionary<TKeyA, IEnumerable<TValue>> GetDictionaryByKeyA()
        //{
        //    lock (_dict)
        //    {
        //        return _dict.GroupBy(g => g.Key).ToDictionary(g => g.Key, g => g.SelectMany(v => v.Value.Select(v1 => v1.Value)));
        //    }
        //}

        public virtual async Task UpdateAsync(TValue value)
        {
            lock (_dict)
            {
                updateDict(value);
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
                    updateDict(value);
            }

            await RaiseUpdated(list);
        }

        private void updateDict(TValue value)
        {
            if (value == null)
                return;

            var keyA = GetKeyA(value);
            if (!_dict.TryGetValue(keyA, out Dictionary<TKeyB, TValue>? dictB))
            {
                dictB = new Dictionary<TKeyB, TValue>();
                _dict.Add(keyA, dictB);
            }

            var keyB = GetKeyB(value);

            //dictB.TryGetValue(keyB, out TValue? prevValue);

            dictB[keyB] = value;

            //if (prevValue != null)
            //    OnElementUpdated(prevValue, value);
        }
    }
}