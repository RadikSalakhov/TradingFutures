namespace TradingFutures.Shared.Data
{
    public struct TradingOrderStatus
    {
        public string Value { get; set; }

        public TradingOrderStatus(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingOrderStatus valueA, TradingOrderStatus valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingOrderStatus valueA, TradingOrderStatus valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingOrderStatus value) => value.Value;

        public static explicit operator TradingOrderStatus(string value) => new(value);

        public static TradingOrderStatus _EMPTY { get => new(string.Empty); }

        public static TradingOrderStatus READY_TO_SUBMIT { get => new("READY_TO_SUBMIT"); }

        public static TradingOrderStatus SUBMITTING { get => new("SUBMITTING"); }

        public static TradingOrderStatus SUBMITTED { get => new("SUBMITTED"); }

        public static TradingOrderStatus PARTIALLY_FILLED { get => new("PARTIALLY_FILLED"); }

        public static TradingOrderStatus PARTIALLY_CANCELLED { get => new("PARTIALLY_CANCELLED"); }

        public static TradingOrderStatus FILLED { get => new("FILLED"); }

        public static TradingOrderStatus CANCELLED { get => new("CANCELLED"); }

        public static TradingOrderStatus CANCELLING { get => new("CANCELLING"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingOrderStatus)
                return false;

            return ((TradingOrderStatus)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}