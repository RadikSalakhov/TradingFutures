using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Server.WorkerHandlers;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Server.Workers
{
    public class HuobiWorker : BackgroundService
    {
        private readonly ILogger<HuobiWorker> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly IWorkerHandler _workerHandler;

        private readonly ICacheService _serverCache;

        private readonly IHuobiClientService _huobiClient;

        private readonly Dictionary<string, Dictionary<TradingOrderSide, TradingTransactionEntity>> _lastTransactionsDict = new();

        public static DateTime LastServerTime { get; private set; }

        public HuobiWorker(ILogger<HuobiWorker> logger,
            IServiceProvider serviceProvider,
            IWorkerHandler workerHandler,
            ICacheService serverCache,
            IHuobiClientService huobiClient)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _workerHandler = workerHandler;
            _serverCache = serverCache;
            _huobiClient = huobiClient;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _workerHandler.RegisterAction<HuobiWorker>(1000, onEvery1000);

            _workerHandler.RegisterAction<HuobiWorker>(60000, onEvery60000);

            _huobiClient.TransactionCompleted += huobiClient_TransactionCompleted;

            await base.StartAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STARTED");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _workerHandler.UnregisteredActions<HuobiWorker>();

            _huobiClient.TransactionCompleted -= huobiClient_TransactionCompleted;

            await base.StopAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STOPPED");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _workerHandler.ProcessStepAsync<HuobiWorker>();
            }
        }

        private async Task onEvery1000()
        {
            LastServerTime = await _huobiClient.GetServerTime();

            var priceTickers = await _huobiClient.GetPriceTickers();

            var tradingPositions = await _huobiClient.GetTradingPositions();
            if (tradingPositions != null)
            {
                if (priceTickers != null)
                {
                    var priceTickersDict = priceTickers.ToDictionary(v => v.Asset, v => v.Price);
                    foreach (var tradingPosition in tradingPositions.Where(v => v.LastPrice == 0m))
                    {
                        if (priceTickersDict.TryGetValue(tradingPosition.Asset, out decimal price))
                            tradingPosition.LastPrice = price;
                    }
                }

                await fillTradingsPositionSettings(tradingPositions);

                await fillTradingPositionsTransactionData(tradingPositions);

                await _serverCache.TradingPosition.UpdateAsync(tradingPositions, clear: true);
            }
        }

        private async Task onEvery60000()
        {
            await _huobiClient.RefreshContractsInfo();
        }

        private Task huobiClient_TransactionCompleted(TradingTransactionEntity arg)
        {
            lock (_lastTransactionsDict)
            {
                _lastTransactionsDict.Clear();
            }
            return Task.CompletedTask;
        }

        private async Task fillTradingsPositionSettings(IEnumerable<TradingPositionEntity> tradingPositions)
        {
            if (tradingPositions == null || !tradingPositions.Any())
                return;

            using var scope = _serviceProvider.CreateScope();

            var tradingPositionSettingsRepositoryService = scope.ServiceProvider.GetRequiredService<ITradingPositionSettingsRepositoryService>();

            var tradingPositionSettingsList = await tradingPositionSettingsRepositoryService.GetAll();
            if (tradingPositionSettingsList == null)
                return;

            var dict = tradingPositionSettingsList.GroupBy(v => v.Asset).ToDictionary(v => v.Key, v => v.ToArray());

            foreach (var tradingPosition in tradingPositions)
            {
                if (dict.TryGetValue(tradingPosition.Asset, out TradingPositionSettingsEntity[]? list))
                {
                    var tradingPositionSettings = list.Where(v => v.OrderSide == tradingPosition.OrderSide).FirstOrDefault();
                    if (tradingPositionSettings != null)
                        tradingPosition.TradingPositionSettings = tradingPositionSettings;
                }

                if (tradingPosition.TradingPositionSettings == null)
                {
                    var key = tradingPosition.ToTradingPositionSettingsKey();
                    if (key.IsValid())
                        tradingPosition.TradingPositionSettings = new TradingPositionSettingsEntity(key);
                }
            }
        }

        private async Task fillTradingPositionsTransactionData(IEnumerable<TradingPositionEntity> tradingPositions)
        {
            var tradingPositionsToProcess = tradingPositions?.Where(v => !v.HasCoins());
            if (tradingPositionsToProcess == null || !tradingPositionsToProcess.Any())
                return;

            await tryInitializeLastTransactionsDict();

            lock (_lastTransactionsDict)
            {
                foreach (var tradingPosition in tradingPositionsToProcess)
                {
                    if (!_lastTransactionsDict.TryGetValue(tradingPosition.Asset, out Dictionary<TradingOrderSide, TradingTransactionEntity>? transactionsDict))
                        continue;

                    if (!transactionsDict.TryGetValue(tradingPosition.OrderSide.GetOpposite(), out TradingTransactionEntity? tradingTransaction))
                        continue;

                    tradingPosition.LastTransactionPrice = tradingTransaction.Price;
                }
            }
        }

        private async Task tryInitializeLastTransactionsDict()
        {
            lock (_lastTransactionsDict)
            {
                if (_lastTransactionsDict.Any())
                    return;
            }

            using var scope = _serviceProvider.CreateScope();

            var tradingTransactionRepositoryService = scope.ServiceProvider.GetRequiredService<ITradingTransactionRepositoryService>();
            var lastTransactionsDict = await tradingTransactionRepositoryService.GetLastTransactions();
            if (lastTransactionsDict.Any())
            {
                lock (_lastTransactionsDict)
                {
                    _lastTransactionsDict.Clear();
                    foreach (var kvp in lastTransactionsDict)
                        _lastTransactionsDict.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}