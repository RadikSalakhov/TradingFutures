using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Configuration;
using TradingFutures.Application.TradingStrategies.Base;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Application.TradingStrategies
{
    public class TradingStrategyA : BaseTradingStrategy
    {
        public TradingStrategyA(
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

        public async Task<bool> Process(TradingPositionEntity tradingPosition, TradingPositionEntity oppositeTradingPosition)
        {
            if (tradingPosition == null || oppositeTradingPosition == null || tradingPosition.TradingPositionSettings == null)
                return false;

            if (!tradingPosition.TradingPositionSettings.TradingBotEnabled)
                return false;

            var serverSettings = CacheService.ServerSettings.Get();

            if (serverSettings.TradingStrategyType != TradingStrategyType.STRATEGY_A)
                return false;

            //Process BUY
            var stepsAmount = tradingPosition.GetStepsAmountInt();
            var oppositeStepsAmount = oppositeTradingPosition.GetStepsAmountInt();

            if (stepsAmount == 0)
            {
                if (await ProcessPositionsBuy(tradingPosition, "INITIAL"))
                    return true;
            }
            else
            {
                if (serverSettings.TradingBotCompensationMode)
                {
                    bool stepsConditionMet;
                    if (serverSettings.TradingBotExceedBuyMode)
                        stepsConditionMet = stepsAmount <= oppositeStepsAmount;
                    else
                        stepsConditionMet = stepsAmount < oppositeStepsAmount;

                    if (stepsConditionMet)
                    {
                        if (await ProcessPositionsBuy(tradingPosition, "COMPENSATION"))
                            return true;
                    }
                }
            }

            //Process SELL
            if (stepsAmount > 0)
            {
                if (await ValidateThreshold(tradingPosition))
                {
                    if (await ProcessPositionsSell(tradingPosition, "PROFIT"))
                        return true;
                }

                if (serverSettings.TradingBotSellLossesMode)
                {
                    if (await ProcessPositionsSell(tradingPosition, "LOSSES", sellLosses: true))
                        return true;
                }
            }

            return false;
        }
    }
}