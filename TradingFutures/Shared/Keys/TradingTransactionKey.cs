using TradingFutures.Shared.Base;
using TradingFutures.Shared.Data;

namespace TradingFutures.Shared.Keys
{
    public class TradingTransactionKey : BaseKey<TradingTransactionKey>
    {
        public DateTime OrderDT { get; set; }

        public string Asset { get; set; } = string.Empty;

        public TradingOrderSide OrderSide { get; set; }

        public TradingTransactionKey()
        {
        }

        public TradingTransactionKey(DateTime orderDT, string asset, TradingOrderSide orderSide)
        {
            OrderDT = orderDT;
            Asset = asset;
            OrderSide = orderSide;
        }

        public override bool Equals(TradingTransactionKey? other)
        {
            return other != null &&
                other.OrderDT == OrderDT &&
                other.Asset == Asset &&
                other.OrderSide == OrderSide;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + OrderDT.GetHashCode();
                hash = hash * 23 + Asset?.GetHashCode() ?? 0;
                hash = hash * 23 + OrderSide.GetHashCode();
                return hash;
            }
        }

        public override bool IsValid()
        {
            return OrderDT != DateTime.MinValue &&
                !string.IsNullOrWhiteSpace(Asset) &&
                (OrderSide == TradingOrderSide.BUY || OrderSide == TradingOrderSide.SELL);
        }

        public override TradingTransactionKey GetCopy()
        {
            return new TradingTransactionKey(OrderDT, Asset, OrderSide);
        }
    }
}