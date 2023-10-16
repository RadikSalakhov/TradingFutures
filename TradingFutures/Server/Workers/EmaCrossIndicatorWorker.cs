using TradingFutures.Application.Abstraction;
using TradingFutures.Server.WorkerHandlers;
using TradingFutures.Shared.Abstraction;

namespace TradingFutures.Server.Workers
{
    public class EmaCrossIndicatorWorker : BackgroundService
    {
        private readonly ILogger<HuobiWorker> _logger;

        private readonly IWorkerHandler _workerHandler;

        private readonly ICacheService _serverCache;

        private readonly ITradingApiClientService _tradingSuiteClientService;

        public EmaCrossIndicatorWorker(ILogger<HuobiWorker> logger,
            IWorkerHandler workerHandler,
            ICacheService serverCache,
            ITradingApiClientService tradingSuiteClientService)
        {
            _logger = logger;
            _workerHandler = workerHandler;
            _serverCache = serverCache;
            _tradingSuiteClientService = tradingSuiteClientService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(1000, onEveryEmaCross_1s);

            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(1000, onEveryEmaCross_1m);
            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(2000, onEveryEmaCross_5m);

            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(3000, onEveryEmaCross_15m);
            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(3000, onEveryEmaCross_30m);
            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(3000, onEveryEmaCross_1h);
            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(3000, onEveryEmaCross_4h);
            _workerHandler.RegisterAction<EmaCrossIndicatorWorker>(3000, onEveryEmaCross_12h);

            await base.StartAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STARTED");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _workerHandler.UnregisteredActions<EmaCrossIndicatorWorker>();

            await base.StopAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STOPPED");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _workerHandler.ProcessStepAsync<EmaCrossIndicatorWorker>();
            }
        }

        private async Task onEveryEmaCross_1s()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("1s");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }

        private async Task onEveryEmaCross_1m()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("1m");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }

        private async Task onEveryEmaCross_5m()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("5m");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }

        private async Task onEveryEmaCross_15m()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("15m");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }

        private async Task onEveryEmaCross_30m()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("30m");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }

        private async Task onEveryEmaCross_1h()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("1h");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }

        private async Task onEveryEmaCross_4h()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("4h");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }

        private async Task onEveryEmaCross_12h()
        {
            var emaCross = await _tradingSuiteClientService.GetEmaCross("12h");
            if (emaCross != null && emaCross.Any())
                await _serverCache.EmaCross.UpdateAsync(emaCross);
        }
    }
}