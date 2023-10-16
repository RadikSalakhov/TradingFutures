namespace TradingFutures.Shared.Utils
{
    public static class MathUtilities
    {
        public static decimal Max(decimal valueA, decimal valueB, decimal valueC)
        {
            return Math.Max(Math.Max(valueA, valueB), valueC);
        }

        public static decimal FloorByPrecision(decimal value, decimal precision)
        {
            return decimal.Floor(value / precision) * precision;
        }

        public static short IncrementClamp(short value, bool incrementUp, short min = -1, short max = 1)
        {
            return (short)Math.Clamp(value + (incrementUp ? 1 : -1), min, max);
        }

        public static int IncrementClamp(int value, bool incrementUp, int min = -1, int max = 1)
        {
            return Math.Clamp(value + (incrementUp ? 1 : -1), min, max);
        }

        public static decimal Lerp(decimal from, decimal to, decimal lerpCoeff)
        {
            lerpCoeff = Math.Clamp(lerpCoeff, 0M, 1M);

            return from * (1M - lerpCoeff) + to * lerpCoeff;
        }

        public static float Lerp(float from, float to, float lerpCoeff)
        {
            lerpCoeff = Math.Clamp(lerpCoeff, 0f, 1f);

            return from * (1f - lerpCoeff) + to * lerpCoeff;
        }

        public static decimal FitClamp(decimal source, decimal fromA, decimal fromB, decimal toA, decimal toB)
        {
            var fit = Fit(source, fromA, fromB, toA, toB);
            return toA <= toB
                ? Math.Clamp(fit, toA, toB)
                : Math.Clamp(fit, toB, toA);
        }

        public static decimal Fit(decimal source, decimal fromA, decimal fromB, decimal toA, decimal toB)
        {
            var deltaA = fromB - fromA;
            if (deltaA == 0M)
                return Lerp(toA, toB, 0.5M);

            var deltaB = toB - toA;
            var scale = deltaB / deltaA;
            var negA = -1 * fromA;
            var offset = (negA * scale) + toA;
            return (source * scale) + offset;
        }

        public static float Fit(float source, float fromA, float fromB, float toA, float toB)
        {
            var deltaA = fromB - fromA;
            if (deltaA == 0f)
                return Lerp(toA, toB, 0.5f);

            var deltaB = toB - toA;
            var scale = deltaB / deltaA;
            var negA = -1 * fromA;
            var offset = (negA * scale) + toA;
            return (source * scale) + offset;
        }
    }
}