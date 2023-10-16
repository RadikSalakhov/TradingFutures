using CryptoExchange.Net.Authentication;
using Huobi.Net.Clients;
using Huobi.Net.Enums;
using Huobi.Net.Objects.Models.UsdtMarginSwap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Application.Configuration;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Infrastructure.HuobiClientServices
{
    public class HuobiClientService : IHuobiClientService
    {
        private const string EMPTY_RESPONSE_ERROR_MESSAGE = "RESPONSE IS EMPTY";
        private const int LEVERAGE_RATE = 10;

        private readonly ILogger<HuobiClientService> _logger;

        private readonly IServiceProvider _serviceProvider;

        protected readonly ICacheService _cacheService;

        private readonly GeneralOptions _generalOptions;

        private readonly HuobiOptions _huobiOptions;

        private readonly ReadOnlyDictionary<string, ContractOptions> _contractOptionsDict;

        private readonly Dictionary<string, HuobiContractInfo> _contractInfoDict;

        public event Func<TradingTransactionEntity, Task>? TransactionCompleted;

        public HuobiClientService(
            ILogger<HuobiClientService> logger,
            IServiceProvider serviceProvider,
            ICacheService cacheService,
            IConfiguration configuration,
            IOptions<GeneralOptions> generalOptions,
            IOptions<HuobiOptions> huobiOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cacheService = cacheService;
            _generalOptions = generalOptions.Value;
            _huobiOptions = huobiOptions.Value;

            _contractOptionsDict = new ReadOnlyDictionary<string, ContractOptions>(ContractOptions.ParseFromConfiguration(configuration));
            _contractInfoDict = new();
        }

        public async Task<DateTime> GetServerTime()
        {
            using var client = new HuobiRestClient();

            var apiCredentials = new ApiCredentials(_huobiOptions.ApiKey, _huobiOptions.ApiSecret);

            client.SetApiCredentials(apiCredentials);

            var result = await client.UsdtMarginSwapApi.ExchangeData.GetServerTimeAsync();
            if (result == null)
            {
                _logger.LogError(result?.Error?.ToString() ?? EMPTY_RESPONSE_ERROR_MESSAGE);
                return DateTime.MinValue;
            }

            return result.Data;
        }

        public async Task<IEnumerable<PriceTickerEntity>> GetPriceTickers()
        {
            using var client = new HuobiRestClient();

            var apiCredentials = new ApiCredentials(_huobiOptions.ApiKey, _huobiOptions.ApiSecret);

            client.SetApiCredentials(apiCredentials);

            var result = await client.UsdtMarginSwapApi.ExchangeData.GetMarketDatasAsync();
            if (result == null || !result.Success || result.Data == null)
            {
                _logger.LogError(result?.Error?.ToString() ?? EMPTY_RESPONSE_ERROR_MESSAGE);
                return Array.Empty<PriceTickerEntity>();
            }

            var resultList = new List<PriceTickerEntity>();

            foreach (var marketData in result.Data)
            {
                if (!marketData.ClosePrice.HasValue)
                    continue;

                if (!_contractOptionsDict.TryGetValue(marketData.ContractCode, out ContractOptions? contractOptions))
                    continue;

                resultList.Add(new PriceTickerEntity
                {
                    ReferenceDT = DateTime.UtcNow,
                    Asset = contractOptions.Asset,
                    Price = marketData.ClosePrice.Value
                });
            }

            return resultList;
        }

        public async Task<IEnumerable<TradingPositionEntity>> GetTradingPositions()
        {
            using var client = new HuobiRestClient();

            var apiCredentials = new ApiCredentials(_huobiOptions.ApiKey, _huobiOptions.ApiSecret);

            client.SetApiCredentials(apiCredentials);

            var result = await client.UsdtMarginSwapApi.Account.GetCrossMarginPositionsAsync();
            if (result == null || !result.Success || result.Data == null)
            {
                _logger.LogError(result?.Error?.ToString() ?? EMPTY_RESPONSE_ERROR_MESSAGE);
                return Array.Empty<TradingPositionEntity>();
            }

            await ensureHuobiContractInfoIsInitialized();

            var resultEntities = new List<TradingPositionEntity>();

            var crossMarginPositions = result.Data.GroupBy(v => v.ContractCode).ToDictionary(v => v.Key, v => v.ToArray());
            foreach (var kvp in _contractOptionsDict)
            {
                var contractCode = kvp.Key;
                if (!isValidHuobiContract(contractCode))
                    continue;

                HuobiPosition? longHuobiPosition = null;
                HuobiPosition? shortHuobiPosition = null;

                if (crossMarginPositions.TryGetValue(contractCode, out HuobiPosition[]? huobiPositions) && huobiPositions.Any())
                {
                    longHuobiPosition = huobiPositions.Where(v => v.Side == OrderSide.Buy).FirstOrDefault();
                    shortHuobiPosition = huobiPositions.Where(v => v.Side == OrderSide.Sell).FirstOrDefault();
                }

                var longPosition = longHuobiPosition != null
                    ? EntitiesConverters.ToTradingPositionEntity(longHuobiPosition)
                    : TradingPositionEntity.Create(Shared.Data.TradingOrderSide.BUY, kvp.Value.Asset);

                var shortPosition = shortHuobiPosition != null
                    ? EntitiesConverters.ToTradingPositionEntity(shortHuobiPosition)
                    : TradingPositionEntity.Create(Shared.Data.TradingOrderSide.SELL, kvp.Value.Asset);

                fillExtraProperties(longPosition);
                fillExtraProperties(shortPosition);

                longPosition.OppositeOrderStepsAmount = shortPosition.GetStepsAmount();
                shortPosition.OppositeOrderStepsAmount = longPosition.GetStepsAmount();

                resultEntities.Add(longPosition);
                resultEntities.Add(shortPosition);
            }

            return resultEntities;
        }

        public async Task RefreshContractsInfo(string? asset)
        {
            string? contractCode = null;
            if (!string.IsNullOrWhiteSpace(asset))
                contractCode = TradingPositionEntity.GetContractCode(asset);

            using var client = new HuobiRestClient();

            var apiCredentials = new ApiCredentials(_huobiOptions.ApiKey, _huobiOptions.ApiSecret);

            client.SetApiCredentials(apiCredentials);

            var result = await client.UsdtMarginSwapApi.ExchangeData.GetContractInfoAsync(contractCode: contractCode);
            if (result == null || !result.Success || result.Data == null)
            {
                _logger.LogError(result?.Error?.ToString() ?? EMPTY_RESPONSE_ERROR_MESSAGE);
                return;
            }

            lock (_contractInfoDict)
            {
                if (contractCode != null)
                {
                    var contractInfo = result.Data.Where(v => v.ContractCode == contractCode).FirstOrDefault();
                    if (contractInfo != null)
                        _contractInfoDict[contractCode] = contractInfo;
                }
                else
                {
                    _contractInfoDict.Clear();

                    foreach (var contractInfo in result.Data)
                    {
                        if (!string.IsNullOrWhiteSpace(contractInfo.ContractCode))
                            _contractInfoDict.Add(contractInfo.ContractCode, contractInfo);
                    }
                }
            }
        }

        public async Task<bool> OpenLong(string asset, string label)
        {
            if (!_generalOptions.IsProduction)
                return true;

            var orderId = await placeOrder(asset, OrderSide.Buy, Offset.Open);

            if (orderId.HasValue)
                await processOrderDetails(asset, orderId.Value, label);

            return orderId.HasValue;
        }

        public async Task<bool> CloseLong(string asset, string label)
        {
            if (!_generalOptions.IsProduction)
                return true;

            var orderId = await placeOrder(asset, OrderSide.Sell, Offset.Close);

            if (orderId.HasValue)
                await processOrderDetails(asset, orderId.Value, label);

            return orderId.HasValue;
        }

        public async Task<bool> OpenShort(string asset, string label)
        {
            if (!_generalOptions.IsProduction)
                return true;

            var orderId = await placeOrder(asset, OrderSide.Sell, Offset.Open);

            if (orderId.HasValue)
                await processOrderDetails(asset, orderId.Value, label);

            return orderId.HasValue;
        }

        public async Task<bool> CloseShort(string asset, string label)
        {
            if (!_generalOptions.IsProduction)
                return true;

            var orderId = await placeOrder(asset, OrderSide.Buy, Offset.Close);

            if (orderId.HasValue)
                await processOrderDetails(asset, orderId.Value, label);

            return orderId.HasValue;
        }

        private async Task ensureHuobiContractInfoIsInitialized()
        {
            bool isInitialzed;
            lock (_contractInfoDict)
            {
                isInitialzed = _contractInfoDict.Any();
            }

            if (!isInitialzed)
                await RefreshContractsInfo(null);
        }

        private bool isValidHuobiContract(string contractCode)
        {
            lock (_contractInfoDict)
            {
                return _contractInfoDict.ContainsKey(contractCode);
            }
        }

        private void fillExtraProperties(TradingPositionEntity tradingPosition)
        {
            if (tradingPosition == null)
                return;

            //Sequence of settings of the properties is important
            lock (_contractInfoDict)
            {
                if (!_contractInfoDict.TryGetValue(tradingPosition.ContractCode, out HuobiContractInfo? contractInfo))
                    return;

                if (contractInfo.ContractSize > 0m)
                    tradingPosition.ContractSize = contractInfo.ContractSize;
            }

            lock (_contractInfoDict)
            {
                if (!_contractOptionsDict.TryGetValue(tradingPosition.ContractCode, out ContractOptions? contractOptions))
                    return;

                tradingPosition.VolumeStep = contractOptions.VolumeStep;
            }

            //AutoBuyThresholdReached
            var serverSettings = _cacheService.ServerSettings.Get();
            var autoBuyThreshold = serverSettings.GetAutoBuyThreshold(tradingPosition.GetStepsAmountInt(), serverSettings.MaxStepsDiff);
            tradingPosition.AutoBuyThresholdReached = tradingPosition.GetPnlOverStepsAmount() < -autoBuyThreshold;
        }

        private async Task<long?> placeOrder(string asset, OrderSide orderSide, Offset offset)
        {
            if (string.IsNullOrWhiteSpace(asset))
                return null;

            var contractCode = TradingPositionEntity.GetContractCode(asset);

            await RefreshContractsInfo(contractCode);

            if (!_contractInfoDict.TryGetValue(contractCode, out HuobiContractInfo? contractInfo))
                return null;

            if (!_contractOptionsDict.TryGetValue(contractCode, out ContractOptions? contractOptions))
                return null;

            var contractSize = contractInfo?.ContractSize ?? 0m;
            if (contractSize <= 0m)
                return null;

            if (contractSize != contractOptions.ContractSize)
                return null;

            var contractVolume = contractOptions.VolumeStep;
            if (contractVolume <= 0m)
                return null;

            using var client = new HuobiRestClient();

            var apiCredentials = new ApiCredentials(_huobiOptions.ApiKey, _huobiOptions.ApiSecret);

            client.SetApiCredentials(apiCredentials);

            var result = await client.UsdtMarginSwapApi.Trading.PlaceCrossMarginOrderAsync(
                contractVolume,
                orderSide,
                LEVERAGE_RATE,
                contractCode: contractCode,
                symbol: contractCode,
                contractType: ContractType.Swap,
                offset: offset,
                orderPriceType: OrderPriceType.Optimal20);

            if (result == null || !result.Success || result.Data == null)
            {
                _logger.LogError(result?.Error?.ToString() ?? EMPTY_RESPONSE_ERROR_MESSAGE);
                return null;
            }

            var orderId = result.Data.OrderId;

            _logger.LogInformation($"Order placed: {orderId}");

            return orderId;
        }

        private async Task<bool> processOrderDetails(string asset, long orderId, string label, int counter = 0)
        {
            if (counter > 5)
                return false;

            await Task.Delay(200);

            if (string.IsNullOrWhiteSpace(asset))
                return false;

            var contractCode = TradingPositionEntity.GetContractCode(asset);

            using var client = new HuobiRestClient();

            var apiCredentials = new ApiCredentials(_huobiOptions.ApiKey, _huobiOptions.ApiSecret);

            client.SetApiCredentials(apiCredentials);

            var result = await client.UsdtMarginSwapApi.Trading.GetCrossMarginOrderDetailsAsync(contractCode, orderId);
            if (result == null || !result.Success || result.Data == null)
            {
                _logger.LogError(result?.Error?.ToString() ?? EMPTY_RESPONSE_ERROR_MESSAGE);
                return false;
            }

            var orderDetails = result.Data;

            if (orderDetails.Status == SwapMarginOrderStatus.Submitting)
                return await processOrderDetails(asset, orderId, label, counter + 1);

            _logger.LogInformation(JsonConvert.SerializeObject(orderDetails));

            var tradingTransaction = EntitiesConverters.ToTradingTransactionEntity(orderDetails);
            tradingTransaction.Label = label;

            var tradingOrderType = tradingTransaction.GetTradingOrderType();
            if (tradingOrderType.IsCLose())
            {
                using var scope = _serviceProvider.CreateScope();
                var tradingTransactionRepositoryService = scope.ServiceProvider.GetRequiredService<ITradingTransactionRepositoryService>();
                await tradingTransactionRepositoryService.CreateOrUpdate(tradingTransaction);
            }

            if (TransactionCompleted != null)
                await TransactionCompleted(tradingTransaction);

            return true;
        }
    }
}