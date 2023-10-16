using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingFutures.Persistence.Base;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.DBModels
{
    [Table("SettingsItem")]
    [PrimaryKey("TypeName", "PropertyName")]
    public class SettingsItemDB : BaseDB<SettingsItemKey, SettingsItemEntity, SettingsItemDB>
    {
        [StringLength(100)]
        public string TypeName { get; set; } = string.Empty;

        [StringLength(50)]
        public string PropertyName { get; set; } = string.Empty;

        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal DecimalValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public string? StringValue { get; set; }

        protected override void FromKeyInternal(SettingsItemKey key)
        {
            TypeName = key.TypeName;
            PropertyName = key.PropertyName;
        }

        protected override SettingsItemKey ToKeyInternal()
        {
            return new SettingsItemKey(TypeName, PropertyName);
        }

        protected override void FromEntityInternal(SettingsItemEntity entity)
        {
            BoolValue = entity.BoolValue;
            IntValue = entity.IntValue;
            DecimalValue = entity.DecimalValue;
            DateTimeValue = entity.DateTimeValue;
            StringValue = entity.StringValue;
        }

        protected override SettingsItemEntity ToEntityInternal()
        {
            return new SettingsItemEntity(ToKey())
            {
                BoolValue = BoolValue,
                IntValue = IntValue,
                DecimalValue = DecimalValue,
                DateTimeValue = DateTimeValue,
                StringValue = StringValue ?? string.Empty
            };
        }
    }
}