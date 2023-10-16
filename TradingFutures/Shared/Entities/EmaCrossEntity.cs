using TradingFutures.Shared.Data;

namespace TradingFutures.Shared.Entities
{
    public class EmaCrossEntity
    {
        public DateTime ReferenceDT { get; set; }

        public string Asset { get; set; }

        public string Interval { get; set; }

        public decimal ValueShort { get; set; }

        public decimal ValueLong { get; set; }

        public decimal PrevValueShort { get; set; }

        public decimal PrevValueLong { get; set; }

        public bool? SlopeShortTrendUp { get; set; }

        public bool? SlopeLongTrendUp { get; set; }

        public decimal GetValueDiff()
        {
            return ValueShort - ValueLong;
        }

        public decimal GetPrevValueDiff()
        {
            return PrevValueShort - PrevValueLong;
        }

        public decimal GetPriceDiffShortCoeff(decimal price)
        {
            return price > 0 ? (price - ValueShort) / price : 0m;
        }

        public decimal GetPriceDiffLongCoeff(decimal price)
        {
            return price > 0 ? (price - ValueLong) / price : 0m;
        }

        public decimal GetCrossDiffCoeff()
        {
            var average = 0.5M * (ValueShort + ValueLong);
            return average > 0 ? GetValueDiff() / average : 0m;
        }

        public decimal GetSlopeShortCoeff()
        {
            if (ValueShort == 0m || PrevValueShort == 0m)
                return 0m;

            var rawCoeff = ValueShort / PrevValueShort;

            return rawCoeff - 1m;
        }

        public decimal GetSlopeLongCoeff()
        {
            if (ValueLong == 0m || PrevValueLong == 0m)
                return 0m;

            var rawCoeff = ValueLong / PrevValueLong;

            return rawCoeff - 1m;
        }

        public bool CheckCondition(TradingConditionEntity? tradingCondition, decimal price)
        {
            if (tradingCondition == null)
                return false;

            if (tradingCondition.IsEmpty())
                return false;

            //SlopeShort
            if (tradingCondition.ValidateSlopeShortMode(GetSlopeShortCoeff()) == false)
                return false;

            //SlopeShortTrend
            if (tradingCondition.ValidateSlopeTrendShortMode(SlopeShortTrendUp) == false)
                return false;

            //SlopeLong
            if (tradingCondition.ValidateSlopeLongMode(GetSlopeLongCoeff()) == false)
                return false;

            //SlopeTrendLong
            if (tradingCondition.ValidateSlopeTrendLongMode(SlopeLongTrendUp) == false)
                return false;

            //CrossDiff
            if (tradingCondition.ValidateCrossDiffMode(GetCrossDiffCoeff()) == false)
                return false;

            //PriceDiffShort
            if (tradingCondition.ValidatePriceDiffShortMode(GetPriceDiffShortCoeff(price)) == false)
                return false;

            //PriceDiffLong
            if (tradingCondition.ValidatePriceDiffLongMode(GetPriceDiffLongCoeff(price)) == false)
                return false;

            return true;
        }

        //public bool CheckCondition(TradingConditionEntity? tradingCondition, decimal price)
        //{
        //    if (tradingCondition == null)
        //        return false;

        //    if (tradingCondition.IsEmpty())
        //        return false;

        //    //SlopeShort
        //    if (tradingCondition.SlopeShortMode != 0)
        //    {
        //        var slopeShortCoeff = GetSlopeShortCoeff();

        //        var valid =
        //            (tradingCondition.SlopeShortMode > 0 && slopeShortCoeff > tradingCondition.SlopeShortThreshold) ||
        //            (tradingCondition.SlopeShortMode < 0 && slopeShortCoeff < tradingCondition.SlopeShortThreshold);

        //        if (!valid)
        //            return false;
        //    }

        //    //SlopeShortTrend
        //    if (tradingCondition.SlopeTrendShortMode != 0)
        //    {
        //        var valid =
        //            (tradingCondition.SlopeTrendShortMode > 0 && SlopeShortTrendUp.HasValue && SlopeShortTrendUp.Value) ||
        //            (tradingCondition.SlopeTrendShortMode < 0 && SlopeShortTrendUp.HasValue && !SlopeShortTrendUp.Value);

        //        if (!valid)
        //            return false;
        //    }

        //    //SlopeLong
        //    if (tradingCondition.SlopeLongMode != 0)
        //    {
        //        var slopeLongCoeff = GetSlopeLongCoeff();

        //        var valid =
        //            (tradingCondition.SlopeLongMode > 0 && slopeLongCoeff > tradingCondition.SlopeLongThreshold) ||
        //            (tradingCondition.SlopeLongMode < 0 && slopeLongCoeff < tradingCondition.SlopeLongThreshold);

        //        if (!valid)
        //            return false;
        //    }

        //    //SlopeTrendLong
        //    if (tradingCondition.SlopeTrendLongMode != 0)
        //    {
        //        var valid =
        //            (tradingCondition.SlopeTrendLongMode > 0 && SlopeLongTrendUp.HasValue && SlopeLongTrendUp.Value) ||
        //            (tradingCondition.SlopeTrendLongMode < 0 && SlopeLongTrendUp.HasValue && !SlopeLongTrendUp.Value);

        //        if (!valid)
        //            return false;
        //    }

        //    //CrossDiff
        //    if (tradingCondition.CrossDiffMode != 0)
        //    {
        //        var crossDiffCoeff = GetCrossDiffCoeff(price);

        //        var valid =
        //            (tradingCondition.CrossDiffMode > 0 && crossDiffCoeff > tradingCondition.CrossDiffThreshold) ||
        //            (tradingCondition.CrossDiffMode < 0 && crossDiffCoeff < tradingCondition.CrossDiffThreshold);

        //        if (!valid)
        //            return false;
        //    }

        //    //PriceDiffShort
        //    if (tradingCondition.PriceDiffShortMode != 0)
        //    {
        //        var priceDiffShortCoeff = GetPriceDiffShortCoeff(price);

        //        var valid =
        //            (tradingCondition.PriceDiffShortMode > 0 && priceDiffShortCoeff > tradingCondition.PriceDiffShortThreshold) ||
        //            (tradingCondition.PriceDiffShortMode < 0 && priceDiffShortCoeff < tradingCondition.PriceDiffShortThreshold);

        //        if (!valid)
        //            return false;
        //    }

        //    //PriceDiffLong
        //    if (tradingCondition.PriceDiffLongMode != 0)
        //    {
        //        var priceDiffLongCoeff = GetPriceDiffLongCoeff(price);

        //        var valid =
        //            (tradingCondition.PriceDiffLongMode > 0 && priceDiffLongCoeff > tradingCondition.PriceDiffLongThreshold) ||
        //            (tradingCondition.PriceDiffLongMode < 0 && priceDiffLongCoeff < tradingCondition.PriceDiffLongThreshold);

        //        if (!valid)
        //            return false;
        //    }

        //    return true;
        //}

        public static EmaCrossEntity Create(string asset, string interval)
        {
            return new EmaCrossEntity
            {
                ReferenceDT = DateTime.UtcNow,
                Asset = asset,
                Interval = interval
            };
        }
    }
}