using Microsoft.AspNetCore.SignalR;
using TradingFutures.Server.Hubs;
using TradingFutures.Server.WorkerHandlers;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Server.Workers
{
    public class NotificationsHubWorker : BackgroundService
    {
        private readonly ILogger<NotificationsHubWorker> _logger;

        private readonly IWorkerHandler _workerHandler;

        private readonly ICacheService _serverCache;

        private readonly IHubContext<NotificationsHub> _hubContext;

        private readonly Queue<Tuple<HubNotificationType, object>> _queueA = new();
        private readonly Queue<Tuple<HubNotificationType, object>> _queueB = new();
        private readonly Queue<Tuple<HubNotificationType, object>> _queueC = new();

        public NotificationsHubWorker(ILogger<NotificationsHubWorker> logger,
            IWorkerHandler workerHandler,
            ICacheService serverCache,
            IHubContext<NotificationsHub> hubContext)
        {
            _logger = logger;
            _workerHandler = workerHandler;
            _serverCache = serverCache;
            _hubContext = hubContext;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _serverCache.ServerSettings.Updated += serverSettings_Updated;
            _serverCache.ServerStatus.Updated += serverStatus_Updated;
            _serverCache.TradingPosition.Updated += tradingPosition_Updated;
            _serverCache.EmaCross.Updated += emaCross_Updated;

            _workerHandler.RegisterAction<NotificationsHubWorker>(10, onEvery10);

            await base.StartAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STARTED");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _serverCache.ServerSettings.Updated -= serverSettings_Updated;
            _serverCache.ServerStatus.Updated -= serverStatus_Updated;
            _serverCache.TradingPosition.Updated -= tradingPosition_Updated;
            _serverCache.EmaCross.Updated -= emaCross_Updated;

            _workerHandler.UnregisteredActions<NotificationsHubWorker>();

            await base.StopAsync(cancellationToken);

            _logger.LogInformation($"{GetType()}: STOPPED");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _workerHandler.ProcessStepAsync<NotificationsHubWorker>();
            }
        }

        private async Task onEvery10()
        {
            {//Queue A
                Tuple<HubNotificationType, object>? tuple;
                lock (_queueA)
                {
                    _queueA.TryDequeue(out tuple);
                }

                if (tuple != null)
                    await _hubContext.Clients.All.SendAsync(tuple.Item1, tuple.Item2);
            }

            {//Queue B
                Tuple<HubNotificationType, object>? tuple;
                lock (_queueB)
                {
                    _queueB.TryDequeue(out tuple);
                }

                if (tuple != null)
                    await _hubContext.Clients.All.SendAsync(tuple.Item1, tuple.Item2);
            }

            {//Queue C
                Tuple<HubNotificationType, object>? tuple;
                lock (_queueC)
                {
                    _queueC.TryDequeue(out tuple);
                }

                if (tuple != null)
                    await _hubContext.Clients.All.SendAsync(tuple.Item1, tuple.Item2);
            }
        }

        private Task serverSettings_Updated(IEnumerable<ServerSettingsEntity> serverSettings)
        {
            if (serverSettings != null && serverSettings.Any())
            {
                lock (_queueA)
                {
                    _queueA.Enqueue(new Tuple<HubNotificationType, object>(HubNotificationType.SERVER_SETTINGS, serverSettings.First()));
                }
            }

            return Task.CompletedTask;
        }

        private Task serverStatus_Updated(IEnumerable<ServerStatusEntity> serverStatus)
        {
            if (serverStatus != null && serverStatus.Any())
            {
                lock (_queueA)
                {
                    _queueA.Enqueue(new Tuple<HubNotificationType, object>(HubNotificationType.SERVER_STATUS, serverStatus.First()));
                }
            }

            return Task.CompletedTask;
        }

        private Task tradingPosition_Updated(IEnumerable<TradingPositionEntity> tradingPsoitions)
        {
            if (tradingPsoitions != null && tradingPsoitions.Any())
            {
                lock (_queueB)
                {
                    _queueB.Enqueue(new Tuple<HubNotificationType, object>(HubNotificationType.TRADING_POSITIONS_COLLECTION, tradingPsoitions));
                }
            }

            return Task.CompletedTask;
        }

        private Task emaCross_Updated(IEnumerable<EmaCrossEntity> emaCross)
        {
            if (emaCross != null && emaCross.Any())
            {
                lock (_queueC)
                {
                    _queueC.Enqueue(new Tuple<HubNotificationType, object>(HubNotificationType.EMA_CROSS_COLLECTION, emaCross));
                }
            }

            return Task.CompletedTask;
        }
    }
}