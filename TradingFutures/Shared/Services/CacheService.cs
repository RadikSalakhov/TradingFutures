using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Services.CacheEntries;

namespace TradingFutures.Shared.Services
{
    public class CacheService : ICacheService
    {
        public EmaCrossCacheEntry EmaCross { get; } = new EmaCrossCacheEntry();

        public ServerSettingsCacheEntry ServerSettings { get; } = new ServerSettingsCacheEntry();

        public ServerStatusCacheEntry ServerStatus { get; } = new ServerStatusCacheEntry();

        public TradingPositionCacheEntry TradingPosition { get; } = new TradingPositionCacheEntry();

        public TradingProfileCacheEntry TradingProfile { get; } = new TradingProfileCacheEntry();
    }
}