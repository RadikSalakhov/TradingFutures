namespace TradingFutures.Shared.Data
{
    public struct TradingPositionMode
    {
        public string Value { get; set; }

        public TradingPositionMode(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static implicit operator string(TradingPositionMode value) => value.Value;

        public static explicit operator TradingPositionMode(string value) => new(value);

        public static TradingPositionMode _EMPTY { get => new(string.Empty); }

        public static TradingPositionMode SINGLE_SIDE { get => new("SINGLE_SIDE"); }

        public static TradingPositionMode DUAL_SIDE { get => new("DUAL_SIDE"); }
    }
}