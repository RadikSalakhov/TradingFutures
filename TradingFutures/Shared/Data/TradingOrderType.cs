namespace TradingFutures.Shared.Data
{
    public struct TradingOrderType
    {
        public string Value { get; set; }

        public TradingOrderType(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingOrderType valueA, TradingOrderType valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingOrderType valueA, TradingOrderType valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingOrderType value) => value.Value;

        public static explicit operator TradingOrderType(string value) => new(value);

        public static TradingOrderType _EMPTY { get => new(string.Empty); }

        public static TradingOrderType OPEN_LONG { get => new("OPEN_LONG"); }

        public static TradingOrderType OPEN_SHORT { get => new("OPEN_SHORT"); }

        public static TradingOrderType CLOSE_LONG { get => new("CLOSE_LONG"); }

        public static TradingOrderType CLOSE_SHORT { get => new("CLOSE_SHORT"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingOrderType)
                return false;

            return ((TradingOrderType)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public bool IsOpen()
        {
            return this == OPEN_LONG || this == OPEN_SHORT;
        }

        public bool IsCLose()
        {
            return this == CLOSE_LONG || this == CLOSE_SHORT;
        }

        public static TradingOrderType ToTradingOrderType(TradingOrderSide orderSide, TradingOrderOffset orderOffset)
        {
            if (orderSide == TradingOrderSide.BUY)
            {
                if (orderOffset == TradingOrderOffset.OPEN)
                {
                    return OPEN_LONG;
                }
                else if (orderOffset == TradingOrderOffset.CLOSE)
                {
                    return CLOSE_SHORT;
                }
            }
            else if (orderSide == TradingOrderSide.SELL)
            {
                if (orderOffset == TradingOrderOffset.OPEN)
                {
                    return OPEN_SHORT;
                }
                else if (orderOffset == TradingOrderOffset.CLOSE)
                {
                    return CLOSE_LONG;
                }
            }

            return _EMPTY;
        }
    }
}