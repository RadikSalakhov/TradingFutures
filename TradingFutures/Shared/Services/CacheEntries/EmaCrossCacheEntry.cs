using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Services.CacheEntries.Base;

namespace TradingFutures.Shared.Services.CacheEntries
{
    public class EmaCrossCacheEntry : BaseDoubleDictCacheEntry<string, string, EmaCrossEntity>
    {
        protected override string GetKeyA(EmaCrossEntity value)
        {
            return value.Asset;
        }

        protected override string GetKeyB(EmaCrossEntity value)
        {
            return value.Interval;
        }
    }
}