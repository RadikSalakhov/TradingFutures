namespace TradingFutures.Shared.Data
{
    public struct TradingConditionType
    {
        public string Value { get; set; }

        public TradingConditionType(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingConditionType valueA, TradingConditionType valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingConditionType valueA, TradingConditionType valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingConditionType value) => value.Value;

        public static explicit operator TradingConditionType(string value) => new(value);

        public static TradingConditionType _EMPTY { get => new(string.Empty); }

        public static TradingConditionType BUY { get => new("BUY"); }

        public static TradingConditionType SELL_PROFIT { get => new("SELL-PROFIT"); }

        public static TradingConditionType SELL_LOSSES { get => new("SELL-LOSSES"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingConditionType)
                return false;

            return ((TradingConditionType)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}