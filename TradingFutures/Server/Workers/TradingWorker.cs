using Microsoft.Extensions.Options;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Application.Configuration;
using TradingFutures.Server.WorkerHandlers;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Server.Workers
{
    public class TradingWorker : BackgroundService
    {
        private readonly ILogger<TradingWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly GeneralOptions _generalOptions;
        private readonly IWorkerHandler _workerHandler;
        private readonly ICacheService _serverCache;
        private readonly ITradingAssistantService _tradingAssistantService;
        private readonly ITradingBotService _tradingBotService;
        private readonly ITelegramService _telegramService;
        private readonly IHuobiClientService _huobiClientService;

        private DateTime _lastCommitePnlDT;
        private decimal? _lastCommitedDayPnl;
        private decimal? _lastCommitedMonthPnl;
        private decimal? _lastCommitedYearPnl;

        public TradingWorker(ILogger<TradingWorker> logger,
            IServiceProvider serviceProvider,
            IOptions<GeneralOptions> generalOptions,
            IWorkerHandler workerHandler,
            ICacheService serverCache,
            ITradingAssistantService tradingAssistantService,
            ITradingBotService tradingBotService,
            ITelegramService telegramService,
            IHuobiClientService huobiClientService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _generalOptions = generalOptions.Value;
            _workerHandler = workerHandler;
            _serverCache = serverCache;
            _tradingAssistantService = tradingAssistantService;
            _tradingBotService = tradingBotService;
            _telegramService = telegramService;
            _huobiClientService = huobiClientService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _workerHandler.RegisterAction<TradingWorker>(500, onEvery500);

            _workerHandler.RegisterAction<TradingWorker>(1000, onEvery1000);

            _workerHandler.RegisterAction<TradingWorker>(2000, onEveryTradingStep);

            _huobiClientService.TransactionCompleted += huobiClientService_TransactionCompleted;

            await base.StartAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STARTED");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _workerHandler.UnregisteredActions<TradingWorker>();

            _huobiClientService.TransactionCompleted -= huobiClientService_TransactionCompleted;

            await base.StopAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STOPPED");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _workerHandler.ProcessStepAsync<TradingWorker>();
            }
        }

        private async Task onEvery500()
        {
            if (_workerHandler.WorkerDT.Date != _lastCommitePnlDT.Date)
            {
                invalidateCachedPNLs();
                _lastCommitePnlDT = _workerHandler.WorkerDT;
            }

            var serverStatus = await getServerStatus();
            if (serverStatus != null)
                await _serverCache.ServerStatus.UpdateAsync(serverStatus);

            await _telegramService.ProcessMessages();
        }

        private async Task onEvery1000()
        {
            using var scope = _serviceProvider.CreateScope();

            var settingsItemRepository = scope.ServiceProvider.GetRequiredService<ISettingsItemRepositoryService>();

            var serverSettings = await settingsItemRepository.LoadServerSettings();
            if (serverSettings != null)
                await _serverCache.ServerSettings.UpdateAsync(serverSettings);
        }

        private async Task onEveryTradingStep()
        {
            using var scope = _serviceProvider.CreateScope();

            var tradingProfileRepository = scope.ServiceProvider.GetRequiredService<ITradingProfileRepositoryService>();
            var tradingProfiles = await tradingProfileRepository.GetAll();
            if (tradingProfiles != null)
                await _serverCache.TradingProfile.UpdateAsync(tradingProfiles, clear: true);

            if (await _tradingAssistantService.Process())
                await Task.Delay(2000);

            if (await _tradingBotService.Process())
                await Task.Delay(3000);
        }

        private Task huobiClientService_TransactionCompleted(TradingTransactionEntity arg)
        {
            invalidateCachedPNLs();
            return Task.CompletedTask;
        }

        private void invalidateCachedPNLs()
        {
            _lastCommitedDayPnl = null;
            _lastCommitedMonthPnl = null;
            _lastCommitedYearPnl = null;
        }

        private async Task<ServerStatusEntity> getServerStatus()
        {
            IServiceScope? scope = null;
            ITradingTransactionRepositoryService? tradingTransactionRepositoryService = null;

            Func<ITradingTransactionRepositoryService> getTradingTransactionRepositoryService =
                () =>
                {
                    if (tradingTransactionRepositoryService == null)
                    {
                        if (scope == null)
                            scope = _serviceProvider.CreateScope();

                        tradingTransactionRepositoryService = scope.ServiceProvider.GetRequiredService<ITradingTransactionRepositoryService>();
                    }

                    return tradingTransactionRepositoryService;
                };

            decimal? commitedDayPnl;
            decimal? commitedMonthPnl;
            decimal? commitedYearPnl;

            try
            {
                commitedDayPnl = _lastCommitedDayPnl;
                if (!commitedDayPnl.HasValue)
                {
                    var dtTuple = DTRangeType.DAY.GetDateTimeMinMaxFromDTRangeType();
                    commitedDayPnl = await getTradingTransactionRepositoryService().GetPnlSum(dtTuple.Item1, dtTuple.Item2);
                    _lastCommitedDayPnl = commitedDayPnl;
                }

                commitedMonthPnl = _lastCommitedMonthPnl;
                if (!commitedMonthPnl.HasValue)
                {
                    var dtTuple = DTRangeType.MONTH.GetDateTimeMinMaxFromDTRangeType();
                    commitedMonthPnl = await getTradingTransactionRepositoryService().GetPnlSum(dtTuple.Item1, dtTuple.Item2);
                    _lastCommitedMonthPnl = commitedMonthPnl;
                }

                commitedYearPnl = _lastCommitedYearPnl;
                if (!commitedYearPnl.HasValue)
                {
                    var dtTuple = DTRangeType.YEAR.GetDateTimeMinMaxFromDTRangeType();
                    commitedYearPnl = await getTradingTransactionRepositoryService().GetPnlSum(dtTuple.Item1, dtTuple.Item2);
                    _lastCommitedYearPnl = commitedYearPnl;
                }
            }
            finally
            {
                if (scope != null)
                    scope.Dispose();
            }

            var result = new ServerStatusEntity
            {
                IsProduction = _generalOptions.IsProduction,
                ServerTime = HuobiWorker.LastServerTime,
                CommitedDayPnl = commitedDayPnl.HasValue ? commitedDayPnl.Value : 0m,
                CommitedMonthPnl = commitedMonthPnl.HasValue ? commitedMonthPnl.Value : 0m,
                CommitedYearPnl = commitedYearPnl.HasValue ? commitedYearPnl.Value : 0m,
            };

            return result;
        }
    }
}