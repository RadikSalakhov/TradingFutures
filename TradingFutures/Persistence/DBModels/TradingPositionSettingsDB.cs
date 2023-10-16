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
    [Table("TradingPositionSettings")]
    [PrimaryKey("Asset", "OrderSide")]
    public class TradingPositionSettingsDB : BaseDB<TradingPositionSettingsKey, TradingPositionSettingsEntity, TradingPositionSettingsDB>
    {
        [StringLength(20)]
        public string Asset { get; set; } = string.Empty;

        [StringLength(10)]
        public string OrderSide { get; set; } = string.Empty;

        public bool AssistantBuyEnabled { get; set; }

        public bool AssistantSellEnabled { get; set; }

        public bool TradingBotEnabled { get; set; }

        public bool EmaCrossLogicOpen { get; set; }

        public bool EmaCrossLogicClose { get; set; }

        protected override void FromKeyInternal(TradingPositionSettingsKey key)
        {
            Asset = key.Asset;
            OrderSide = key.OrderSide;
        }

        protected override TradingPositionSettingsKey ToKeyInternal()
        {
            return new TradingPositionSettingsKey(Asset, (TradingOrderSide)OrderSide);
        }

        protected override void FromEntityInternal(TradingPositionSettingsEntity entity)
        {
            AssistantBuyEnabled = entity.AssistantBuyEnabled;
            AssistantSellEnabled = entity.AssistantSellEnabled;
            TradingBotEnabled = entity.TradingBotEnabled;
            EmaCrossLogicOpen = entity.EmaCrossLogicOpen;
            EmaCrossLogicClose = entity.EmaCrossLogicClose;
        }

        protected override TradingPositionSettingsEntity ToEntityInternal()
        {
            return new TradingPositionSettingsEntity(ToKey())
            {
                AssistantBuyEnabled = AssistantBuyEnabled,
                AssistantSellEnabled = AssistantSellEnabled,
                TradingBotEnabled = TradingBotEnabled,
                EmaCrossLogicOpen = EmaCrossLogicOpen,
                EmaCrossLogicClose = EmaCrossLogicClose
            };
        }
    }
}