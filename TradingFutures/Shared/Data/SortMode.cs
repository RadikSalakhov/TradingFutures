namespace TradingFutures.Shared.Data
{
    public struct SortMode
    {
        public string Value { get; set; }

        public SortMode(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(SortMode valueA, SortMode valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(SortMode valueA, SortMode valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(SortMode value) => value.Value;

        public static explicit operator SortMode(string value) => new(value);

        public static SortMode _EMPTY { get => new(string.Empty); }

        public static SortMode PNL { get => new("PNL"); }

        public static SortMode PNL_NORMALIZED { get => new("PNL_NORMALIZED"); }

        public override bool Equals(object? obj)
        {
            if (obj is not SortMode)
                return false;

            return ((SortMode)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}