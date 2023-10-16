namespace TradingFutures.Shared.Data
{
    public struct TradingOrderOffset
    {
        public string Value { get; set; }

        public TradingOrderOffset(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingOrderOffset valueA, TradingOrderOffset valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingOrderOffset valueA, TradingOrderOffset valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingOrderOffset value) => value.Value;

        public static explicit operator TradingOrderOffset(string value) => new(value);

        public static TradingOrderOffset _EMPTY { get => new(string.Empty); }

        public static TradingOrderOffset OPEN { get => new("OPEN"); }

        public static TradingOrderOffset CLOSE { get => new("CLOSE"); }

        public static TradingOrderOffset BOTH { get => new("BOTH"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingOrderOffset)
                return false;

            return ((TradingOrderOffset)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}