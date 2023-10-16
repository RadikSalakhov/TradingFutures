namespace TradingFutures.Shared.Data
{
    public struct TradingProfileId
    {
        public string Value { get; set; }

        public TradingProfileId(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(TradingProfileId valueA, TradingProfileId valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(TradingProfileId valueA, TradingProfileId valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(TradingProfileId value) => value.Value;

        public static explicit operator TradingProfileId(string value) => new(value);

        public static TradingProfileId _EMPTY { get => new(string.Empty); }

        public static TradingProfileId ASSISTANT { get => new("ASSISTANT"); }

        public static TradingProfileId PROFILE_A { get => new("PROFILE-A"); }

        public static TradingProfileId PROFILE_B { get => new("PROFILE-B"); }

        public static TradingProfileId PROFILE_C { get => new("PROFILE-C"); }

        public static TradingProfileId PROFILE_D { get => new("PROFILE-D"); }

        public static TradingProfileId PROFILE_E { get => new("PROFILE-E"); }

        public static TradingProfileId STRATEGY_C_1M { get => new("STRATEGY-C-1M"); }

        public static TradingProfileId STRATEGY_C_5M { get => new("STRATEGY-C-5M"); }

        public static TradingProfileId STRATEGY_C_15M { get => new("STRATEGY-C-15M"); }

        public override bool Equals(object? obj)
        {
            if (obj is not TradingProfileId)
                return false;

            return ((TradingProfileId)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public string GetShortName()
        {
            if (this == ASSISTANT) return "ASS";
            if (this == PROFILE_A) return "A";
            if (this == PROFILE_B) return "B";
            if (this == PROFILE_C) return "C";
            if (this == PROFILE_D) return "D";
            if (this == PROFILE_E) return "E";
            if (this == STRATEGY_C_1M) return "ST-C-1M";
            if (this == STRATEGY_C_5M) return "ST-C-5M";
            if (this == STRATEGY_C_15M) return "ST-C-15M";

            return string.Empty;
        }

        public static IEnumerable<TradingProfileId> GetAll()
        {
            yield return ASSISTANT;
            yield return PROFILE_A;
            yield return PROFILE_B;
            yield return PROFILE_C;
            yield return PROFILE_D;
            yield return PROFILE_E;
            yield return STRATEGY_C_1M;
            yield return STRATEGY_C_5M;
            yield return STRATEGY_C_15M;
        }

        public static IEnumerable<TradingProfileId> GetMain()
        {
            yield return PROFILE_A;
            yield return PROFILE_B;
            yield return PROFILE_C;
            yield return PROFILE_D;
            yield return PROFILE_E;
        }
    }
}