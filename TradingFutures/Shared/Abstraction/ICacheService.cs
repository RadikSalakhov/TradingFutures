using TradingFutures.Shared.Services.CacheEntries;

namespace TradingFutures.Shared.Abstraction
{
    public interface ICacheService
    {
        EmaCrossCacheEntry EmaCross { get; }

        ServerSettingsCacheEntry ServerSettings { get; }

        ServerStatusCacheEntry ServerStatus { get; }

        TradingPositionCacheEntry TradingPosition { get; }

        TradingProfileCacheEntry TradingProfile { get; }
    }
}