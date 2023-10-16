namespace TradingFutures.Shared.Data
{
    public struct TradingMarginMode
    {
        public string Value { get; set; }

        public TradingMarginMode(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static implicit operator string(TradingMarginMode value) => value.Value;

        public static explicit operator TradingMarginMode(string value) => new(value);

        public static TradingMarginMode _EMPTY { get => new(string.Empty); }

        public static TradingMarginMode CROSS { get => new("CROSS"); }

        public static TradingMarginMode ISOLATED { get => new("ISOLATED"); }

        public static TradingMarginMode ALL { get => new("ALL"); }
    }
}