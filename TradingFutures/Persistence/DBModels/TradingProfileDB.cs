using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingFutures.Persistence.Base;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.DBModels
{
    [Table("TradingProfiles")]
    public class TradingProfileDB : BaseDB<TradingProfileKey, TradingProfileEntity, TradingProfileDB>
    {
        [Key, StringLength(20)]
        public string TradingProfileId { get; set; } = string.Empty;

        [ForeignKey("TradingProfileId")]
        public ICollection<TradingConditionDB>? TradingConditions { get; set; }

        protected override void FromKeyInternal(TradingProfileKey key)
        {
            TradingProfileId = key.TradingProfileId;
        }

        protected override TradingProfileKey ToKeyInternal()
        {
            return new TradingProfileKey(TradingProfileId);
        }

        protected override void FromEntityInternal(TradingProfileEntity entity)
        {
        }

        protected override TradingProfileEntity ToEntityInternal()
        {
            var result = new TradingProfileEntity(ToKey());

            if (TradingConditions != null && TradingConditions.Any())
                result.SetTradingConditions(TradingConditions.Select(v => v.ToEntity()));

            return result;
        }
    }
}