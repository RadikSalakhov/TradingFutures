using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Services.CacheEntries.Base;

namespace TradingFutures.Shared.Services.CacheEntries
{
    public class TradingProfileCacheEntry : BaseDictCacheEntry<string, TradingProfileEntity>
    {
        protected override string GetKey(TradingProfileEntity value)
        {
            return value.TradingProfileId;
        }

        public IEnumerable<TradingConditionEntity> GetTradingConditions(string tradingProfileId, TradingConditionType conditionType)
        {
            var tradingProfile = GetByKey(tradingProfileId);
            if (tradingProfile == null || tradingProfile.TradingConditions == null)
                return Array.Empty<TradingConditionEntity>();

            return tradingProfile.TradingConditions
                .Where(v => v.ConditionType == conditionType)
                .Where(v => !v.IsEmpty());
        }
    }
}