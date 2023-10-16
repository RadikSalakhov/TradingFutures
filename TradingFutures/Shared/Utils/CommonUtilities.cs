using System;

namespace TradingFutures.Shared.Utils
{
    public static class CommonUtilities
    {
        public static string GetDecimalsDisplayFormat(int decimals)
        {
            return decimals > 0
                ? $"0.{string.Concat(Enumerable.Repeat("0", decimals))}"
                : "0";
        }

        public static int GetAdaptiveDecimalsAmount(decimal value)
        {
            value = Math.Abs(value);
            if (value >= 1000m) return 0;
            else if (value >= 100m) return 1;
            else if (value >= 10m) return 2;
            else if (value >= 1m) return 3;
            else if (value >= 0.1m) return 4;
            else if (value >= 0.01m) return 5;
            else if (value >= 0.001m) return 6;
            else
                return 7;
        }

        public static string GetDefaultDisplayFormat(int decimals, bool includeSign)
        {
            var format = GetDecimalsDisplayFormat(decimals);

            return includeSign
                ? $"+{format};-{format}"
                : format;
        }

        public static string GetPercentageDisplayFormat(int decimals, bool includeSign)
        {
            var format = GetDecimalsDisplayFormat(decimals);

            return includeSign
                ? $"+{format}%;-{format}%"
                : $"{format}%";
        }

        public static string GetCurrencyDisplayFormat(int decimals, bool includeSign)
        {
            var format = GetDecimalsDisplayFormat(decimals);

            return includeSign
                ? $"+{format} $;-{format} $"
                : $"{format} $";
        }

        public static string ToDefaultString(decimal value, int decimals = -1, bool includeSign = false)
        {
            if (decimals < 0)
                decimals = GetAdaptiveDecimalsAmount(value);

            var format = GetDefaultDisplayFormat(decimals, includeSign);
            return value.ToString(format);
        }

        public static string ToPercentageString(decimal value, int decimals = -1, bool includeSign = false)
        {
            if (decimals < 0)
                decimals = GetAdaptiveDecimalsAmount(value);

            var format = GetPercentageDisplayFormat(decimals, includeSign);
            return value.ToString(format);
        }

        public static string ToCurrencyString(decimal value, int decimals = -1, bool includeSign = false)
        {
            if (decimals < 0)
                decimals = GetAdaptiveDecimalsAmount(value);

            var format = GetCurrencyDisplayFormat(decimals, includeSign);
            return value.ToString(format);
        }

        public static DateTime TruncateMillisecods(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        }

        public static string SwitchString(short value, string plus, string minus, string zero)
        {
            if (value > 0) return plus;
            if (value < 0) return minus;
            return zero;
        }
    }
}