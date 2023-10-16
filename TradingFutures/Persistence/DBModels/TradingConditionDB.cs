using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingFutures.Persistence.Base;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.DBModels
{
    [Table("TradingConditions")]
    [PrimaryKey("TradingProfileId", "ConditionType", "Interval")]
    public class TradingConditionDB : BaseDB<TradingConditionKey, TradingConditionEntity, TradingConditionDB>
    {
        [ForeignKey("TradingProfile"), StringLength(20)]
        public string TradingProfileId { get; set; } = string.Empty;

        [StringLength(20)]
        public string ConditionType { get; set; } = string.Empty;

        [StringLength(20)]
        public string Interval { get; set; } = string.Empty;

        public short SlopeShortMode { get; set; }
        public short SlopeTrendShortMode { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal SlopeShortThreshold { get; set; }

        public short SlopeLongMode { get; set; }
        public short SlopeTrendLongMode { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal SlopeLongThreshold { get; set; }

        public short CrossDiffMode { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal CrossDiffThreshold { get; set; }

        public short PriceDiffShortMode { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal PriceDiffShortThreshold { get; set; }

        public short PriceDiffLongMode { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal PriceDiffLongThreshold { get; set; }

        public TradingProfileDB? TradingProfile { get; set; }

        protected override void FromKeyInternal(TradingConditionKey key)
        {
            TradingProfileId = key.TradingProfileId;
            ConditionType = key.ConditionType;
            Interval = key.Interval;
        }

        protected override TradingConditionKey ToKeyInternal()
        {
            return new TradingConditionKey(TradingProfileId, new TradingConditionType(ConditionType), new TradingInterval(Interval));
        }

        protected override void FromEntityInternal(TradingConditionEntity entity)
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

        protected override TradingConditionEntity ToEntityInternal()
        {
            return new TradingConditionEntity(ToKey())
            {
                SlopeShortMode = SlopeShortMode,
                SlopeTrendShortMode = SlopeTrendShortMode,
                SlopeShortThreshold = SlopeShortThreshold,

                SlopeLongMode = SlopeLongMode,
                SlopeTrendLongMode = SlopeTrendLongMode,
                SlopeLongThreshold = SlopeLongThreshold,

                CrossDiffMode = CrossDiffMode,
                CrossDiffThreshold = CrossDiffThreshold,

                PriceDiffShortMode = PriceDiffShortMode,
                PriceDiffShortThreshold = PriceDiffShortThreshold,

                PriceDiffLongMode = PriceDiffLongMode,
                PriceDiffLongThreshold = PriceDiffLongThreshold
            };
        }
    }
}