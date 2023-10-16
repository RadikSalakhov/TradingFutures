using TradingFutures.Shared.Base;

namespace TradingFutures.Shared.Keys
{
    public class TradingProfileKey : BaseKey<TradingProfileKey>
    {
        public string TradingProfileId { get; set; } = string.Empty;

        public TradingProfileKey()
        {
        }

        public TradingProfileKey(string tradingProfileId)
        {
            TradingProfileId = tradingProfileId;
        }

        public override bool Equals(TradingProfileKey? other)
        {
            return other != null &&
                other.TradingProfileId == TradingProfileId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + TradingProfileId.GetHashCode();
                return hash;
            }
        }

        public override bool IsValid()
        {
            return TradingProfileId != string.Empty;
        }

        public override TradingProfileKey GetCopy()
        {
            return new TradingProfileKey(TradingProfileId);
        }
    }
}