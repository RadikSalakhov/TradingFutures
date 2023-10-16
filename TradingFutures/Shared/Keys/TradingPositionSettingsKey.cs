using TradingFutures.Shared.Base;
using TradingFutures.Shared.Data;

namespace TradingFutures.Shared.Keys
{
    public class TradingPositionSettingsKey : BaseKey<TradingPositionSettingsKey>
    {
        public string Asset { get; set; } = string.Empty;

        public TradingOrderSide OrderSide { get; set; }

        public TradingPositionSettingsKey()
        {
        }

        public TradingPositionSettingsKey(string asset, TradingOrderSide orderSide)
        {
            Asset = asset;
            OrderSide = orderSide;
        }

        public override bool Equals(TradingPositionSettingsKey? other)
        {
            return other != null &&
                other.Asset == Asset &&
                other.OrderSide == OrderSide;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Asset?.GetHashCode() ?? 0;
                hash = hash * 23 + OrderSide.GetHashCode();
                return hash;
            }
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Asset) && (OrderSide == TradingOrderSide.BUY || OrderSide == TradingOrderSide.SELL);
        }

        public override TradingPositionSettingsKey GetCopy()
        {
            return new TradingPositionSettingsKey(Asset, OrderSide);
        }
    }
}