namespace TradingFutures.Shared.Data
{
    public struct TradingStrategyType
    {
        public string Value { get; set; }

        public TradingStrategyType(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingStrategyType valueA, TradingStrategyType valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingStrategyType valueA, TradingStrategyType valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingStrategyType value) => value.Value;

        public static explicit operator TradingStrategyType(string value) => new(value);

        public static TradingStrategyType _EMPTY { get => new(string.Empty); }

        public static TradingStrategyType STRATEGY_A { get => new("STRATEGY-A"); }

        public static TradingStrategyType STRATEGY_B { get => new("STRATEGY-B"); }

        public static TradingStrategyType STRATEGY_C { get => new("STRATEGY-C"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingStrategyType)
                return false;

            return ((TradingStrategyType)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public static IEnumerable<TradingStrategyType> GetAll()
        {
            yield return STRATEGY_A;
            yield return STRATEGY_B;
            yield return STRATEGY_C;
        }

        public string GetShortName()
        {
            if (this == STRATEGY_A) return "ST-A";
            if (this == STRATEGY_B) return "ST-B";
            if (this == STRATEGY_C) return "ST-C";

            return string.Empty;
        }
    }
}