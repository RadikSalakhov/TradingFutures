using System.ComponentModel.DataAnnotations.Schema;
using TradingFutures.Shared.Base;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Shared.Entities
{
    public class SettingsItemEntity : BaseEntity<SettingsItemKey, SettingsItemEntity>
    {
        public string TypeName
        {
            get => Key.TypeName;
            set => Key.TypeName = value;
        }

        public string PropertyName
        {
            get => Key.PropertyName;
            set => Key.PropertyName = value;
        }

        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal DecimalValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public string StringValue { get; set; } = string.Empty;

        public SettingsItemEntity()
        {
        }

        public SettingsItemEntity(SettingsItemKey key)
            : base(key)
        {
        }

        protected override SettingsItemEntity CreateInstanceInternal(SettingsItemKey key)
        {
            return new SettingsItemEntity(key);
        }

        protected override void CopyFromInternal(SettingsItemEntity entity)
        {
            BoolValue = entity.BoolValue;
            IntValue = entity.IntValue;
            DecimalValue = entity.DecimalValue;
            DateTimeValue = entity.DateTimeValue;
            StringValue = entity.StringValue;
        }
    }
}