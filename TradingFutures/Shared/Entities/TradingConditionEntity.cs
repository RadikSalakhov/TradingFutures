using System.Diagnostics;
using TradingFutures.Shared.Base;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Shared.Entities
{
    public class TradingConditionEntity : BaseEntity<TradingConditionKey, TradingConditionEntity>
    {
        public string TradingProfileId
        {
            get => Key.TradingProfileId;
            set => Key.TradingProfileId = value;
        }

        public TradingConditionType ConditionType
        {
            get => Key.ConditionType;
            set => Key.ConditionType = value;
        }

        public TradingInterval Interval
        {
            get => Key.Interval;
            set => Key.Interval = value;
        }

        public short SlopeShortMode { get; set; }
        public short SlopeTrendShortMode { get; set; }
        public decimal SlopeShortThreshold { get; set; }

        public short SlopeLongMode { get; set; }
        public short SlopeTrendLongMode { get; set; }
        public decimal SlopeLongThreshold { get; set; }

        public short CrossDiffMode { get; set; }
        public decimal CrossDiffThreshold { get; set; }

        public short PriceDiffShortMode { get; set; }
        public decimal PriceDiffShortThreshold { get; set; }

        public short PriceDiffLongMode { get; set; }
        public decimal PriceDiffLongThreshold { get; set; }

        public TradingConditionEntity()
        {
        }

        public TradingConditionEntity(TradingConditionKey key)
            : base(key)
        {
        }

        protected override TradingConditionEntity CreateInstanceInternal(TradingConditionKey key)
        {
            return new TradingConditionEntity(key);
        }

        protected override void CopyFromInternal(TradingConditionEntity entity)
        {
            SlopeShortMode = entity.SlopeShortMode;
            SlopeTrendShortMode = entity.SlopeTrendShortMode;
            SlopeShortThreshold = entity.SlopeShortThreshold;

            SlopeLongMode = entity.SlopeLongMode;
            SlopeTrendLongMode = entity.SlopeTrendLongMode;
            SlopeLongThreshold = entity.SlopeLongThreshold;

            CrossDiffMode = entity.CrossDiffMode;
            CrossDiffThreshold = entity.CrossDiffThreshold;

            PriceDiffShortMode = entity.PriceDiffShortMode;
            PriceDiffShortThreshold = entity.PriceDiffShortThreshold;

            PriceDiffLongMode = entity.PriceDiffLongMode;
            PriceDiffLongThreshold = entity.PriceDiffLongThreshold;
        }

        public bool IsEmpty()
        {
            return
                SlopeShortMode == 0 &&
                SlopeTrendShortMode == 0 &&
                SlopeLongMode == 0 &&
                SlopeTrendLongMode == 0 &&
                CrossDiffMode == 0 &&
                PriceDiffShortMode == 0 &&
                PriceDiffLongMode == 0;
        }

        public TradingConditionEntity GetCopyOpposite()
        {
            var result = GetCopy();

            result.SlopeShortMode *= -1;
            result.SlopeTrendShortMode *= -1;
            result.SlopeShortThreshold *= -1M;

            result.SlopeLongMode *= -1;
            result.SlopeTrendLongMode *= -1;
            result.SlopeLongThreshold *= -1M;

            result.CrossDiffMode *= -1;
            result.CrossDiffThreshold *= -1M;

            result.PriceDiffShortMode *= -1;
            result.PriceDiffShortThreshold *= -1M;

            result.PriceDiffLongMode *= -1;
            result.PriceDiffLongThreshold *= -1M;

            return result;
        }

        public bool? ValidateSlopeShortMode(decimal slopeShortCoeff)
        {
            if (SlopeShortMode == 0M)
                return null;

            return
                (SlopeShortMode > 0 && slopeShortCoeff > SlopeShortThreshold) ||
                (SlopeShortMode < 0 && slopeShortCoeff < SlopeShortThreshold);
        }

        public bool? ValidateSlopeTrendShortMode(bool? slopeShortTrendUp)
        {
            if (SlopeTrendShortMode == 0M)
                return null;

            return
                (SlopeTrendShortMode > 0 && slopeShortTrendUp.HasValue && slopeShortTrendUp.Value) ||
                (SlopeTrendShortMode < 0 && slopeShortTrendUp.HasValue && !slopeShortTrendUp.Value);
        }

        public bool? ValidateSlopeLongMode(decimal slopeLongCoeff)
        {
            if (SlopeLongMode == 0M)
                return null;

            return
                (SlopeLongMode > 0 && slopeLongCoeff > SlopeLongThreshold) ||
                (SlopeLongMode < 0 && slopeLongCoeff < SlopeLongThreshold);
        }

        public bool? ValidateSlopeTrendLongMode(bool? slopeLongTrendUp)
        {
            if (SlopeTrendLongMode == 0M)
                return null;

            return
                (SlopeTrendLongMode > 0 && slopeLongTrendUp.HasValue && slopeLongTrendUp.Value) ||
                (SlopeTrendLongMode < 0 && slopeLongTrendUp.HasValue && !slopeLongTrendUp.Value);
        }

        public bool? ValidateCrossDiffMode(decimal crossDiffCoeff)
        {
            if (CrossDiffMode == 0M)
                return null;

            return
                (CrossDiffMode > 0 && crossDiffCoeff > CrossDiffThreshold) ||
                (CrossDiffMode < 0 && crossDiffCoeff < CrossDiffThreshold);
        }

        public bool? ValidatePriceDiffShortMode(decimal priceDiffShortCoeff)
        {
            if (PriceDiffShortMode == 0M)
                return null;

            return
                (PriceDiffShortMode > 0 && priceDiffShortCoeff > PriceDiffShortThreshold) ||
                (PriceDiffShortMode < 0 && priceDiffShortCoeff < PriceDiffShortThreshold);
        }

        public bool? ValidatePriceDiffLongMode(decimal priceDiffLongCoeff)
        {
            if (PriceDiffLongMode == 0M)
                return null;

            return
                (PriceDiffLongMode > 0 && priceDiffLongCoeff > PriceDiffLongThreshold) ||
                (PriceDiffLongMode < 0 && priceDiffLongCoeff < PriceDiffLongThreshold);
        }
    }
}