namespace TradingFutures.Shared.Data
{
    public struct TradingOrderSide
    {
        public string Value { get; set; }

        public TradingOrderSide(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingOrderSide valueA, TradingOrderSide valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingOrderSide valueA, TradingOrderSide valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingOrderSide value) => value.Value;

        public static explicit operator TradingOrderSide(string value) => new(value);

        public static TradingOrderSide _EMPTY { get => new(string.Empty); }

        public static TradingOrderSide BUY { get => new("BUY"); }

        public static TradingOrderSide SELL { get => new("SELL"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingOrderSide)
                return false;

            return ((TradingOrderSide)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public TradingOrderSide GetOpposite()
        {
            if (this == BUY) return SELL;
            if (this == SELL) return BUY;
            return _EMPTY;
        }
    }
}