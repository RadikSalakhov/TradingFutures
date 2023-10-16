using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Configuration;
using TradingFutures.Application.TradingStrategies;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;

namespace TradingFutures.Application.Services
{
    public class TradingBotService : ITradingBotService
    {
        private readonly ICacheService _cacheService;

        private readonly TradingStrategyA _tradingStrategyA;
        private readonly TradingStrategyB _tradingStrategyB;
        private readonly TradingStrategyC _tradingStrategyC;

        private DateTime _lastProcessDT = DateTime.MinValue;

        public TradingBotService(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IOptions<GeneralOptions> generalOptions,
            ICacheService cacheService,
            IHuobiClientService huobiClientService)
        {
            _cacheService = cacheService;

            _tradingStrategyA = new TradingStrategyA(
                serviceProvider,
                configuration,
                generalOptions,
                cacheService,
                huobiClientService);

            _tradingStrategyB = new TradingStrategyB(
                serviceProvider,
                configuration,
                generalOptions,
                cacheService,
                huobiClientService);

            _tradingStrategyC = new TradingStrategyC(
                serviceProvider,
                configuration,
                generalOptions,
                cacheService,
                huobiClientService);
        }

        public async Task<bool> Process()
        {
            var serverSettings = _cacheService.ServerSettings.Get();
            if (!serverSettings.TradingBotEnabled)
                return false;

            var processed = false;

            var tradingPositionsDict = _cacheService.TradingPosition.GetFirstDictReversed();

            var maxLastCacheUpdateDT = _lastProcessDT;

            foreach (var kvp in tradingPositionsDict)
            {
                var longTradingPosition = kvp.Value[TradingOrderSide.BUY];
                var shortTradingPosition = kvp.Value[TradingOrderSide.SELL];

                if (longTradingPosition == null || shortTradingPosition == null)
                    continue;

                if (longTradingPosition.TradingPositionSettings != null && longTradingPosition.LastCacheUpdateDT > _lastProcessDT)
                {
                    if (longTradingPosition.LastCacheUpdateDT > maxLastCacheUpdateDT)
                        maxLastCacheUpdateDT = longTradingPosition.LastCacheUpdateDT;

                    if (longTradingPosition.TradingPositionSettings.TradingBotEnabled)
                    {
                        if (serverSettings.TradingStrategyType == TradingStrategyType.STRATEGY_A)
                        {
                            if (await _tradingStrategyA.Process(longTradingPosition, shortTradingPosition))
                                processed = true;
                        }
                        else if (serverSettings.TradingStrategyType == TradingStrategyType.STRATEGY_B)
                        {
                            if (await _tradingStrategyB.Process(longTradingPosition, shortTradingPosition))
                                processed = true;
                        }
                        else if (serverSettings.TradingStrategyType == TradingStrategyType.STRATEGY_C)
                        {
                            if (await _tradingStrategyC.Process(longTradingPosition, shortTradingPosition))
                                processed = true;
                        }
                    }
                }

                if (shortTradingPosition.TradingPositionSettings != null && shortTradingPosition.LastCacheUpdateDT > _lastProcessDT)
                {
                    if (shortTradingPosition.LastCacheUpdateDT > maxLastCacheUpdateDT)
                        maxLastCacheUpdateDT = shortTradingPosition.LastCacheUpdateDT;

                    if (shortTradingPosition.TradingPositionSettings.TradingBotEnabled)
                    {
                        if (serverSettings.TradingStrategyType == TradingStrategyType.STRATEGY_A)
                        {
                            if (await _tradingStrategyA.Process(shortTradingPosition, longTradingPosition))
                                processed = true;
                        }
                        else if (serverSettings.TradingStrategyType == TradingStrategyType.STRATEGY_B)
                        {
                            if (await _tradingStrategyB.Process(shortTradingPosition, longTradingPosition))
                                processed = true;
                        }
                        else if (serverSettings.TradingStrategyType == TradingStrategyType.STRATEGY_C)
                        {
                            if (await _tradingStrategyC.Process(shortTradingPosition, longTradingPosition))
                                processed = true;
                        }
                    }
                }
            }

            _lastProcessDT = maxLastCacheUpdateDT;

            return processed;
        }
    }
}