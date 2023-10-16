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
    public class TradingStrategyC : BaseTradingStrategy
    {
        public TradingStrategyC(
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
            if (tradingPosition == null || tradingPosition.TradingPositionSettings == null || oppositeTradingPosition == null)
                return false;

            if (!tradingPosition.TradingPositionSettings.TradingBotEnabled)
                return false;

            var serverSettings = CacheService.ServerSettings.Get();

            if (serverSettings.TradingStrategyType != TradingStrategyType.STRATEGY_C)
                return false;

            await tryResetEmaCrossLogic(tradingPosition);

            if (await tryBuyInitial(tradingPosition))
                return true;

            if (await tryBuyByIntervals(tradingPosition, oppositeTradingPosition))
                return true;

            if (await trySellByIntervals(tradingPosition, oppositeTradingPosition))
                return true;

            if (await trySellWithCompensation(tradingPosition, oppositeTradingPosition))
                return true;

            if (await trySellWithProfit(tradingPosition, oppositeTradingPosition))
                return true;

            if (await trySellWithLosses(tradingPosition))
                return true;

            return false;
        }

        private bool validateStepsCondition(TradingPositionEntity tradingPosition, TradingPositionEntity oppositeTradingPosition, bool validateForBuy)
        {
            var serverSettings = CacheService.ServerSettings.Get();

            var stepsAmount = tradingPosition.GetStepsAmountInt();
            var oppositeStepsAmount = oppositeTradingPosition.HasCoins() ? oppositeTradingPosition.GetStepsAmountInt() : 0;
            var stepsAmountDiff = stepsAmount - oppositeStepsAmount;

            if (validateForBuy)
            {
                if (serverSettings.TradingBotExceedBuyMode)
                    return stepsAmountDiff < serverSettings.MaxStepsDiff;
                else
                    return stepsAmountDiff < 0;
            }
            else
            {
                if (serverSettings.TradingBotExceedSellMode)
                    return stepsAmountDiff > -serverSettings.MaxStepsDiff;
                else
                    return stepsAmountDiff > 0;
            }
        }

        private async Task<bool> tryBuyInitial(TradingPositionEntity tradingPosition)
        {
            if (tradingPosition.HasCoins())
                return false;

            if (await ProcessPositionsBuy(tradingPosition, "INITIAL"))
            {
                await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings!.Key, emaCrossLogicOpen: false, emaCrossLogicClose: false);
                return true;
            }

            return false;
        }

        private async Task<bool> tryBuyByIntervals(TradingPositionEntity tradingPosition, TradingPositionEntity oppositeTradingPosition)
        {
            if (!tradingPosition.HasCoins())
                return false;

            if (!validateStepsCondition(tradingPosition, oppositeTradingPosition, validateForBuy: true))
                return false;

            if (!tradingPosition.AutoBuyThresholdReached)
                return false;

            var allowBuy = false;
            if (oppositeTradingPosition.HasCoins())
            {
                if (tradingPosition.IsLongPosition())
                    allowBuy = tradingPosition.LastPrice < oppositeTradingPosition.CostOpen;
                else if (tradingPosition.IsShortPosition())
                    allowBuy = tradingPosition.LastPrice > oppositeTradingPosition.CostOpen;
            }
            else
                allowBuy = true;

            if (!allowBuy)
                return false;

            if (tradingPosition.TradingPositionSettings!.EmaCrossLogicOpen)
            {
                if (await ProcessPositionsBuy(tradingPosition, "INT-1M", overrideTradingProfileId: TradingProfileId.STRATEGY_C_1M))
                {
                    await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings.Key, emaCrossLogicOpen: false);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> trySellByIntervals(TradingPositionEntity tradingPosition, TradingPositionEntity oppositeTradingPosition)
        {
            var stepsAmount = tradingPosition.GetStepsAmountInt();
            if (stepsAmount <= 1)
                return false;

            if (!validateStepsCondition(tradingPosition, oppositeTradingPosition, validateForBuy: false))
                return false;

            if (!await ValidateThreshold(tradingPosition))
                return false;

            if (tradingPosition.TradingPositionSettings!.EmaCrossLogicClose)
            {
                if (await ProcessPositionsSell(tradingPosition, "INT-1M", overrideTradingProfileId: TradingProfileId.STRATEGY_C_1M))
                {
                    await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings.Key, emaCrossLogicClose: false);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> trySellWithCompensation(TradingPositionEntity tradingPosition, TradingPositionEntity oppositeTradingPosition)
        {
            var stepsAmount = tradingPosition.GetStepsAmountInt();
            if (stepsAmount <= 1)
                return false;

            var serverSettings = CacheService.ServerSettings.Get();
            if (!serverSettings.TradingBotCompensationMode)
                return false;

            if (validateStepsCondition(tradingPosition, oppositeTradingPosition, validateForBuy: true))
                return false;

            if (tradingPosition.TradingPositionSettings!.EmaCrossLogicClose)
            {
                if (await ProcessPositionsSell(tradingPosition, "COMPENSATION", overrideTradingProfileId: TradingProfileId.STRATEGY_C_1M))
                {
                    await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings.Key, emaCrossLogicClose: false);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> trySellWithProfit(TradingPositionEntity tradingPosition, TradingPositionEntity oppositeTradingPosition)
        {
            var stepsAmount = tradingPosition.GetStepsAmountInt();
            if (stepsAmount <= 1)
                return false;

            if (!validateStepsCondition(tradingPosition, oppositeTradingPosition, validateForBuy: false))
                return false;

            if (!await ValidateThreshold(tradingPosition))
                return false;

            if (await ProcessPositionsSell(tradingPosition, "PROFIT"))
                return true;

            return false;
        }

        private async Task<bool> trySellWithLosses(TradingPositionEntity tradingPosition)
        {
            var stepsAmount = tradingPosition.GetStepsAmountInt();
            if (stepsAmount <= 1)
                return false;

            var serverSettings = CacheService.ServerSettings.Get();
            if (!serverSettings.TradingBotSellLossesMode)
                return false;

            if (await ProcessPositionsSell(tradingPosition, "LOSSES", sellLosses: true))
                return true;

            return false;
        }

        private async Task tryResetEmaCrossLogic(TradingPositionEntity tradingPosition)
        {
            var emaCross = CacheService.EmaCross.GetByKeys(tradingPosition.Asset, TradingInterval.Interval_1M);
            if (emaCross == null)
                return;

            const decimal THRESHOLD_COEFF = 1.0M / 1000M;

            var currentCrossDiffCoeff = emaCross.GetCrossDiffCoeff();

            if (tradingPosition.IsLongPosition())
            {
                if (!tradingPosition.TradingPositionSettings!.EmaCrossLogicOpen && currentCrossDiffCoeff > THRESHOLD_COEFF)
                    await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings.Key, emaCrossLogicOpen: true);

                if (!tradingPosition.TradingPositionSettings.EmaCrossLogicClose && currentCrossDiffCoeff < -THRESHOLD_COEFF)
                    await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings.Key, emaCrossLogicClose: true);
            }

            if (tradingPosition.IsShortPosition())
            {
                if (!tradingPosition.TradingPositionSettings!.EmaCrossLogicOpen && currentCrossDiffCoeff < -THRESHOLD_COEFF)
                    await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings.Key, emaCrossLogicOpen: true);

                if (!tradingPosition.TradingPositionSettings.EmaCrossLogicClose && currentCrossDiffCoeff > THRESHOLD_COEFF)
                    await UpdateEmaCrossLogicProperties(tradingPosition.TradingPositionSettings.Key, emaCrossLogicClose: true);
            }
        }
    }
}