using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using TradingFutures.Shared.Base;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Shared.Entities
{
    public class TradingPositionSettingsEntity : BaseEntity<TradingPositionSettingsKey, TradingPositionSettingsEntity>
    {
        public string Asset
        {
            get => Key.Asset;
            set => Key.Asset = value;
        }

        public TradingOrderSide OrderSide
        {
            get => Key.OrderSide;
            set => Key.OrderSide = value;
        }

        public bool AssistantBuyEnabled { get; set; }

        public bool AssistantSellEnabled { get; set; }

        public bool TradingBotEnabled { get; set; }

        public bool EmaCrossLogicOpen { get; set; }

        public bool EmaCrossLogicClose { get; set; }

        public TradingPositionSettingsEntity()
        {
        }

        public TradingPositionSettingsEntity(TradingPositionSettingsKey key)
            : base(key)
        {
        }

        protected override TradingPositionSettingsEntity CreateInstanceInternal(TradingPositionSettingsKey key)
        {
            return new TradingPositionSettingsEntity(key);
        }

        protected override void CopyFromInternal(TradingPositionSettingsEntity entity)
        {
            AssistantBuyEnabled = entity.AssistantBuyEnabled;
            AssistantSellEnabled = entity.AssistantSellEnabled;
            TradingBotEnabled = entity.TradingBotEnabled;
            EmaCrossLogicOpen = entity.EmaCrossLogicOpen;
            EmaCrossLogicClose = entity.EmaCrossLogicClose;
        }
    }
}