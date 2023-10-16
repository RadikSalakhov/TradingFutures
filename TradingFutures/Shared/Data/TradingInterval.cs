namespace TradingFutures.Shared.Data
{
    public struct TradingInterval
    {
        public string Value { get; set; }

        public TradingInterval(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingInterval valueA, TradingInterval valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingInterval valueA, TradingInterval valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingInterval value) => value.Value;

        public static explicit operator TradingInterval(string value) => new(value);

        public static TradingInterval _EMPTY { get => new(string.Empty); }

        public static TradingInterval Interval_1S { get => new("1S"); }
        public static TradingInterval Interval_1M { get => new("1M"); }
        public static TradingInterval Interval_5M { get => new("5M"); }
        public static TradingInterval Interval_15M { get => new("15M"); }
        public static TradingInterval Interval_30M { get => new("30M"); }
        public static TradingInterval Interval_1H { get => new("1H"); }
        public static TradingInterval Interval_4H { get => new("4H"); }
        public static TradingInterval Interval_12H { get => new("12H"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingInterval)
                return false;

            return ((TradingInterval)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public static IEnumerable<TradingInterval> GetAll()
        {
            yield return Interval_1S;
            yield return Interval_1M;
            yield return Interval_5M;
            yield return Interval_15M;
            yield return Interval_30M;
            yield return Interval_1H;
            yield return Interval_4H;
            yield return Interval_12H;
        }
    }
}