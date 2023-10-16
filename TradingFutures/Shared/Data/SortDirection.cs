namespace TradingFutures.Shared.Data
{
    public struct SortDirection
    {
        public string Value { get; set; }

        public SortDirection(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(SortDirection valueA, SortDirection valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(SortDirection valueA, SortDirection valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(SortDirection value) => value.Value;

        public static explicit operator SortDirection(string value) => new(value);

        public static SortDirection _EMPTY { get => new(string.Empty); }

        public static SortDirection ASC { get => new("ASC"); }

        public static SortDirection DESC { get => new("DESC"); }

        public override bool Equals(object? obj)
        {
            if (obj is not SortDirection)
                return false;

            return ((SortDirection)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}