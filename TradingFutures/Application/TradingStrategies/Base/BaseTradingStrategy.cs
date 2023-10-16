using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Application.Configuration;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Application.TradingStrategies.Base
{
    public abstract class BaseTradingStrategy
    {
        protected IServiceProvider ServiceProvider { get; }

        protected GeneralOptions GeneralOptions { get; }

        protected ICacheService CacheService { get; }

        protected IHuobiClientService HuobiClientService { get; }

        public BaseTradingStrategy(IServiceProvider serviceProvider,
            IConfiguration configuration,
            IOptions<GeneralOptions> generalOptions,
            ICacheService cacheService,
            IHuobiClientService huobiClientService)
        {
            ServiceProvider = serviceProvider;
            GeneralOptions = generalOptions.Value;
            CacheService = cacheService;
            HuobiClientService = huobiClientService;
        }

        protected Task<bool> ValidateThreshold(TradingPositionEntity tradingPosition)
        {
            var minPnl = CacheService.ServerSettings.Get().MinPnl;
            if (minPnl < 0m)
                return Task.FromResult(false);

            return Task.FromResult(tradingPosition.IsPnlThreasholdReached(minPnl));
        }

        protected async Task<bool> ProcessPositionsBuy(TradingPositionEntity tradingPosition, string label, TradingProfileId? overrideTradingProfileId = null, bool dryRun = false)
        {
            var tradingProfileId = overrideTradingProfileId.HasValue
                ? overrideTradingProfileId.Value
                : CacheService.ServerSettings.Get().TradingProfileId;
            if (tradingProfileId == TradingProfileId._EMPTY)
                return false;

            var buyConditions = CacheService.TradingProfile.GetTradingConditions(tradingProfileId, TradingConditionType.BUY);

            if (!checkConditions(tradingPosition, buyConditions))
                return false;

            if (dryRun)
                return true;

            var result = false;

            if (tradingPosition.IsLongPosition())
                result = await HuobiClientService.OpenLong(tradingPosition.Asset, label);

            if (tradingPosition.IsShortPosition())
                result = await HuobiClientService.OpenShort(tradingPosition.Asset, label);

            if (result)
                await ResetAssistantBuySellEnabled(tradingPosition.ToTradingPositionSettingsKey());

            return result;
        }

        protected async Task<bool> ProcessPositionsSell(TradingPositionEntity tradingPosition, string label, TradingProfileId? overrideTradingProfileId = null, bool sellLosses = false, bool dryRun = false)
        {
            var tradingProfileId = overrideTradingProfileId.HasValue
                ? overrideTradingProfileId.Value
                : CacheService.ServerSettings.Get().TradingProfileId;
            if (tradingProfileId == TradingProfileId._EMPTY)
                return false;

            var tradingConditionType = sellLosses ? TradingConditionType.SELL_LOSSES : TradingConditionType.SELL_PROFIT;

            var sellConditions = CacheService.TradingProfile.GetTradingConditions(tradingProfileId, tradingConditionType);

            if (!checkConditions(tradingPosition, sellConditions))
                return false;

            if (dryRun)
                return true;

            var result = false;

            if (tradingPosition.IsLongPosition())
                result = await HuobiClientService.CloseLong(tradingPosition.Asset, label);

            if (tradingPosition.IsShortPosition())
                result = await HuobiClientService.CloseShort(tradingPosition.Asset, label);

            if (result)
                await ResetAssistantBuySellEnabled(tradingPosition.ToTradingPositionSettingsKey());

            return result;
        }

        protected async Task<bool> ResetAssistantBuySellEnabled(TradingPositionSettingsKey key)
        {
            if (key == null || !key.IsValid())
                return false;

            using var scope = ServiceProvider.CreateScope();

            var tradingPositionSettingsRepository = scope.ServiceProvider.GetRequiredService<ITradingPositionSettingsRepositoryService>();

            var tradingPositionSettings = await tradingPositionSettingsRepository.UpdateProperties(key,
                    (entity) =>
                    {
                        entity.AssistantBuyEnabled = false;
                        entity.AssistantSellEnabled = false;
                    });

            return
                tradingPositionSettings?.AssistantBuyEnabled == false &&
                tradingPositionSettings?.AssistantSellEnabled == false;
        }

        protected async Task UpdateEmaCrossLogicProperties(TradingPositionSettingsKey key, bool? emaCrossLogicOpen = null, bool? emaCrossLogicClose = null)
        {
            if (key == null || !key.IsValid())
                return;

            using var scope = ServiceProvider.CreateScope();

            var tradingPositionSettingsRepository = scope.ServiceProvider.GetRequiredService<ITradingPositionSettingsRepositoryService>();

            await tradingPositionSettingsRepository.UpdateProperties(key,
                (entity) =>
                {
                    if (emaCrossLogicOpen.HasValue)
                        entity.EmaCrossLogicOpen = emaCrossLogicOpen.Value;

                    if (emaCrossLogicClose.HasValue)
                        entity.EmaCrossLogicClose = emaCrossLogicClose.Value;
                });
        }

        private bool checkConditions(TradingPositionEntity tradingPosition, IEnumerable<TradingConditionEntity> conditions)
        {
            if (!conditions.Any())
                return false;

            foreach (var condition in conditions)
            {
                var emaCross = CacheService.EmaCross.GetByKeys(tradingPosition.Asset, condition.Interval);
                if (emaCross == null)
                    return false;

                TradingConditionEntity conditionToCheck;
                if (tradingPosition.IsLongPosition())
                    conditionToCheck = condition;
                else if (tradingPosition.IsShortPosition())
                    conditionToCheck = condition.GetCopyOpposite();
                else
                    return false;

                if (!emaCross.CheckCondition(conditionToCheck, tradingPosition.LastPrice))
                    return false;
            }

            return true;
        }

        //private bool checkBuyConditions(TradingPositionEntity tradingPosition, IEnumerable<TradingConditionEntity> buyConditions)
        //{
        //    if (!buyConditions.Any())
        //        return false;

        //    foreach (var buyCondition in buyConditions)
        //    {
        //        var emaCross = CacheService.EmaCross.GetByKeys(tradingPosition.Asset, buyCondition.Interval);
        //        if (emaCross == null)
        //            return false;

        //        //LONG ORDER
        //        if (tradingPosition.PositionType == TradingPositionType.LONG)
        //        {
        //            if (!emaCross.CheckConditionsRising(buyCondition))
        //                return false;
        //        }

        //        //SHORT ORDER
        //        if (tradingPosition.PositionType == TradingPositionType.SHORT)
        //        {
        //            if (!emaCross.CheckConditionsFalling(buyCondition))
        //                return false;
        //        }
        //    }

        //    return true;
        //}

        //private bool checkSellConditions(TradingPositionEntity tradingPosition, IEnumerable<TradingConditionEntity> sellConditions)
        //{
        //    if (!sellConditions.Any())
        //        return false;

        //    foreach (var sellCondition in sellConditions)
        //    {
        //        var emaCross = CacheService.EmaCross.GetByKeys(tradingPosition.Asset, sellCondition.Interval);
        //        if (emaCross == null)
        //            return false;

        //        //LONG ORDER
        //        if (tradingPosition.PositionType == TradingPositionType.LONG)
        //        {
        //            if (!emaCross.CheckConditionsFalling(sellCondition))
        //                return false;
        //        }

        //        //SHORT ORDER
        //        if (tradingPosition.PositionType == TradingPositionType.SHORT)
        //        {
        //            if (!emaCross.CheckConditionsRising(sellCondition))
        //                return false;
        //        }
        //    }

        //    return true;
        //}
    }
}