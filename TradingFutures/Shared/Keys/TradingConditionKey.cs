using TradingFutures.Shared.Base;
using TradingFutures.Shared.Data;

namespace TradingFutures.Shared.Keys
{
    public class TradingConditionKey : BaseKey<TradingConditionKey>
    {
        public string TradingProfileId { get; set; } = string.Empty;

        public TradingConditionType ConditionType { get; set; }

        public TradingInterval Interval { get; set; } = TradingInterval._EMPTY;

        public TradingConditionKey()
        {
        }

        public TradingConditionKey(string tradingProfileId, TradingConditionType conditionType, TradingInterval interval)
        {
            TradingProfileId = tradingProfileId;
            ConditionType = conditionType;
            Interval = interval;
        }

        public override bool Equals(TradingConditionKey? other)
        {
            return other != null &&
                other.TradingProfileId == TradingProfileId &&
                other.ConditionType == ConditionType &&
                other.Interval == Interval;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + TradingProfileId.GetHashCode();
                hash = hash * 23 + ConditionType.GetHashCode();
                hash = hash * 23 + Interval.Value?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public override bool IsValid()
        {
            return
                !string.IsNullOrWhiteSpace(TradingProfileId) &&
                ConditionType != TradingConditionType._EMPTY &&
                !string.IsNullOrWhiteSpace(Interval);
        }

        public override TradingConditionKey GetCopy()
        {
            return new TradingConditionKey(TradingProfileId, ConditionType, Interval);
        }
    }
}