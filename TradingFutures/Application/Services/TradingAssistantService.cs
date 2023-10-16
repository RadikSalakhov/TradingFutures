using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Configuration;
using TradingFutures.Application.TradingStrategies.Base;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Services;

namespace TradingFutures.Application.Services
{
    public class TradingAssistantService : BaseTradingStrategy, ITradingAssistantService
    {
        public TradingAssistantService(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IOptions<GeneralOptions> generalOptions,
            ICacheService cacheService,
            IHuobiClientService huobiClientService)
            : base(serviceProvider,
                  configuration,
                  generalOptions,
                  cacheService,
                  huobiClientService)
        {
        }

        public async Task<bool> Process()
        {
            var serverSettings = CacheService.ServerSettings.Get();
            if (!serverSettings.TradingAssistantEnabled)
                return false;

            var tradingPositionsDict = CacheService.TradingPosition.GetFirstDictReversed();

            foreach (var kvp in tradingPositionsDict)
            {
                var longTradingPosition = kvp.Value[TradingOrderSide.BUY];
                var shortTradingPosition = kvp.Value[TradingOrderSide.SELL];

                if (longTradingPosition == null || shortTradingPosition == null)
                    continue;

                if (await processTradingPositions(longTradingPosition, shortTradingPosition))
                    return true;

                if (await processTradingPositions(shortTradingPosition, longTradingPosition))
                    return true;
            }

            return false;
        }

        private async Task<bool> processTradingPositions(TradingPositionEntity tradingPosition, TradingPositionEntity oppositeTradingPosition)
        {
            if (tradingPosition == null || tradingPosition.TradingPositionSettings == null || oppositeTradingPosition == null)
                return false;

            if (!tradingPosition.TradingPositionSettings.AssistantBuyEnabled && !tradingPosition.TradingPositionSettings.AssistantSellEnabled)
                return false;

            if (tradingPosition.TradingPositionSettings.AssistantBuyEnabled)
            {
                var stepsAmount = tradingPosition.GetStepsAmountInt();
                var oppositeStepsAmount = oppositeTradingPosition.GetStepsAmountInt();

                if (!await ProcessPositionsBuy(tradingPosition, "ASSISTANT", overrideTradingProfileId: TradingProfileId.ASSISTANT, dryRun: true))
                    return false;

                if (!await ResetAssistantBuySellEnabled(tradingPosition.ToTradingPositionSettingsKey()))
                    return false;

                await ProcessPositionsBuy(tradingPosition, "ASSISTANT", overrideTradingProfileId: TradingProfileId.ASSISTANT);

                return true;
            }

            if (tradingPosition.TradingPositionSettings.AssistantSellEnabled)
            {
                if (!await ProcessPositionsSell(tradingPosition, "ASSISTANT", overrideTradingProfileId: TradingProfileId.ASSISTANT, dryRun: true))
                    return false;

                if (!await ResetAssistantBuySellEnabled(tradingPosition.ToTradingPositionSettingsKey()))
                    return false;

                await ProcessPositionsSell(tradingPosition, "ASSISTANT", overrideTradingProfileId: TradingProfileId.ASSISTANT);

                return true;
            }

            return false;
        }
    }
}