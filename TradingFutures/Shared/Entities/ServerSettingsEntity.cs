using TradingFutures.Shared.Base;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Keys;
using TradingFutures.Shared.Utils;

namespace TradingFutures.Shared.Entities
{
    public class ServerSettingsEntity : BaseEntity<ServerSettingsKey, ServerSettingsEntity>
    {
        private const decimal PNL_MIN = 0.1m;
        private const decimal PNL_STEP = 0.1m;

        private const decimal AUTOBUY_THRESHOLD_STEP = 0.1m;

        private decimal _minPnl;

        public bool TradingAssistantEnabled { get; set; }

        public bool TradingBotEnabled { get; set; }

        public bool TradingBotExceedBuyMode { get; set; }

        public bool TradingBotExceedSellMode { get; set; }

        public bool TradingBotCompensationMode { get; set; }

        public bool TradingBotSellLossesMode { get; set; }

        public bool TelegramNotificationsEnabled { get; set; }

        public string TradingStrategyType { get; set; } = string.Empty;

        public string TradingProfileId { get; set; } = string.Empty;

        public decimal MinPnl
        {
            get => _minPnl > PNL_MIN ? _minPnl : PNL_MIN;
            set => _minPnl = value;
        }

        public int MaxStepsDiff { get; set; }

        public decimal AutoBuyThresholdMin { get; set; }

        public decimal AutoBuyThresholdMax { get; set; }

        public ServerSettingsEntity()
        {
        }

        public ServerSettingsEntity(ServerSettingsKey key)
            : base(key)
        {
        }

        protected override ServerSettingsEntity CreateInstanceInternal(ServerSettingsKey key)
        {
            return new ServerSettingsEntity(key);
        }

        protected override void CopyFromInternal(ServerSettingsEntity entity)
        {
            TradingAssistantEnabled = entity.TradingAssistantEnabled;
            TradingBotEnabled = entity.TradingBotEnabled;
            TradingBotExceedBuyMode = entity.TradingBotExceedBuyMode;
            TradingBotExceedSellMode = entity.TradingBotExceedSellMode;
            TradingBotCompensationMode = entity.TradingBotCompensationMode;
            TradingBotSellLossesMode = entity.TradingBotSellLossesMode;
            TelegramNotificationsEnabled = entity.TelegramNotificationsEnabled;
            TradingStrategyType = entity.TradingStrategyType;
            TradingProfileId = entity.TradingProfileId;
            MinPnl = entity.MinPnl;
            MaxStepsDiff = entity.MaxStepsDiff;
            AutoBuyThresholdMin = entity.AutoBuyThresholdMin;
            AutoBuyThresholdMax = entity.AutoBuyThresholdMax;
        }

        public decimal GetAutoBuyThreshold(int stepsAmount, int maxStepsAmount)
        {
            return MathUtilities.FitClamp(stepsAmount, 0, maxStepsAmount, AutoBuyThresholdMin, AutoBuyThresholdMax);
        }

        public void IncrementMinPnlCoeff(bool incrementUp)
        {
            if (incrementUp)
                _minPnl += PNL_STEP;
            else
                _minPnl = Math.Max(_minPnl - PNL_STEP, PNL_MIN);
        }

        public void IncrementMaxStepsDiff(bool incrementUp)
        {
            if (incrementUp)
                MaxStepsDiff += 1;
            else
                MaxStepsDiff = Math.Max(MaxStepsDiff - 1, 1);
        }

        public void IncrementAutoBuyThresholdMin(bool incrementUp)
        {
            if (incrementUp)
                AutoBuyThresholdMin += AUTOBUY_THRESHOLD_STEP;
            else
                AutoBuyThresholdMin = Math.Max(AutoBuyThresholdMin - AUTOBUY_THRESHOLD_STEP, AUTOBUY_THRESHOLD_STEP);
        }

        public void IncrementAutoBuyThresholdMax(bool incrementUp)
        {
            if (incrementUp)
                AutoBuyThresholdMax += AUTOBUY_THRESHOLD_STEP;
            else
                AutoBuyThresholdMax = Math.Max(AutoBuyThresholdMax - AUTOBUY_THRESHOLD_STEP, AUTOBUY_THRESHOLD_STEP);
        }

        public string GetTradingStrategyTypeShortName()
        {
            return new TradingStrategyType(TradingStrategyType).GetShortName();
        }

        public string GetTradingProfileIdShortName()
        {
            return new TradingProfileId(TradingProfileId).GetShortName();
        }

        public void IncrementTradingBotStrategy(bool incrementUp)
        {
            var strategyTypes = Data.TradingStrategyType.GetAll().ToList();
            if (!strategyTypes.Any())
                return;

            var index = strategyTypes.IndexOf(new TradingStrategyType(TradingStrategyType));
            if (index >= 0)
            {
                index = MathUtilities.IncrementClamp(index, incrementUp, 0, strategyTypes.Count() - 1);
                TradingStrategyType = strategyTypes[index];
            }
            else
                TradingStrategyType = strategyTypes[0];
        }

        public void IncrementTradingProfile(bool incrementUp)
        {
            var tradingProfiles = Data.TradingProfileId.GetMain().ToList();
            if (!tradingProfiles.Any())
                return;

            var index = tradingProfiles.IndexOf(new TradingProfileId(TradingProfileId));
            if (index >= 0)
            {
                index = MathUtilities.IncrementClamp(index, incrementUp, 0, tradingProfiles.Count() - 1);
                TradingProfileId = tradingProfiles[index];
            }
            else
                TradingProfileId = tradingProfiles[0];
        }
    }
}