using System;

namespace TradingFutures.Shared.Data
{
    public struct DTRangeType
    {
        public string Value { get; set; }

        public DTRangeType(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(DTRangeType valueA, DTRangeType valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(DTRangeType valueA, DTRangeType valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(DTRangeType value) => value.Value;

        public static explicit operator DTRangeType(string value) => new(value);

        public static DTRangeType _EMPTY { get => new(string.Empty); }

        public static DTRangeType MINUTE { get => new("MINUTE"); }

        public static DTRangeType HOUR { get => new("HOUR"); }

        public static DTRangeType DAY { get => new("DAY"); }

        public static DTRangeType MONTH { get => new("MONTH"); }

        public static DTRangeType YEAR { get => new("YEAR"); }

        public override bool Equals(object? obj)
        {
            if (obj is not DTRangeType)
                return false;

            return ((DTRangeType)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public string GetDTRangeTypeName()
        {
            if (this == MINUTE) return "m";
            if (this == HOUR) return "H";
            if (this == DAY) return "D";
            if (this == MONTH) return "M";
            if (this == YEAR) return "Y";
            return string.Empty;
        }

        public string GetDTRangeTypeShortDescription(DateTime dt)
        {
            if (this == MINUTE) return $"{dt:HH:mm}";
            if (this == HOUR) return $"{dt:HH}";
            if (this == DAY) return $"{dt:dd.MM}";
            if (this == MONTH) return $"{dt:MMM}";
            if (this == YEAR) return $"{dt:yyyy}";
            return string.Empty;
        }

        public DateTime GetUtcNowForDTRangeType()
        {
            var utcNow = DateTime.UtcNow;

            if (this == MINUTE) return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, 0);
            if (this == HOUR) return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 0, 0);
            if (this == DAY) return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day);
            if (this == MONTH) return new DateTime(utcNow.Year, utcNow.Month, 1);
            if (this == YEAR) return new DateTime(utcNow.Year, 1, 1);

            return utcNow;
        }

        public DateTime GetDateTimeFromDTRangeType(DateTime source, int offset)
        {
            if (this == MINUTE) return source.AddMinutes(offset);
            if (this == HOUR) return source.AddHours(offset);
            if (this == DAY) return source.AddDays(offset);
            if (this == MONTH) return source.AddMonths(offset);
            if (this == YEAR) return source.AddYears(offset);

            return source;
        }

        public Tuple<DateTime, DateTime> GetDateTimeMinMaxFromDTRangeType(int offset = 0)
        {
            var refDT = GetUtcNowForDTRangeType();

            var minDT = GetDateTimeFromDTRangeType(refDT, offset);
            var maxDT = GetDateTimeFromDTRangeType(refDT, offset + 1);

            return new Tuple<DateTime, DateTime>(minDT, maxDT);
        }
    }
}