using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Client.Hubs
{
    public class HubClient : IHubClient
    {
        private const int TIMEOUT_MS = 5000;

        private readonly NavigationManager _navigationManager;

        private readonly ICacheService _cacheService;

        private HubConnection? _hubConnection;

        private DateTime _lastNotificationDT = DateTime.UtcNow;

        private DateTime _lastReconnectAttempt = DateTime.UtcNow;

        public event Func<Task>? StateChanged;

        public HubConnectionState HubConnectionState
        {
            get
            {
                return _hubConnection?.State ?? HubConnectionState.Disconnected;
            }
        }

        public HubClient(NavigationManager navigationManager, ICacheService cacheService)
        {
            _navigationManager = navigationManager;
            _cacheService = cacheService;
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                _hubConnection.Closed -= hubConnection_Closed;
                _hubConnection.Reconnecting -= hubConnection_Reconnecting;
                _hubConnection.Reconnected -= hubConnection_Reconnected;

                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async Task StartAsync()
        {
            if (_hubConnection != null)
                await _hubConnection.DisposeAsync();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("hub"))
                .Build();

            _hubConnection.Closed += hubConnection_Closed;
            _hubConnection.Reconnecting += hubConnection_Reconnecting;
            _hubConnection.Reconnected += hubConnection_Reconnected;

            _hubConnection.On<ServerSettingsEntity>(HubNotificationType.SERVER_SETTINGS,
                async entity =>
                {
                    _lastNotificationDT = DateTime.UtcNow;
                    await _cacheService.ServerSettings.UpdateAsync(entity);
                });

            _hubConnection.On<ServerStatusEntity>(HubNotificationType.SERVER_STATUS,
                async entity =>
                {
                    _lastNotificationDT = DateTime.UtcNow;
                    await _cacheService.ServerStatus.UpdateAsync(entity);
                });

            _hubConnection.On<IEnumerable<TradingPositionEntity>>(HubNotificationType.TRADING_POSITIONS_COLLECTION,
                async entities =>
                {
                    _lastNotificationDT = DateTime.UtcNow;
                    await _cacheService.TradingPosition.UpdateAsync(entities, clear: true); _lastNotificationDT = DateTime.UtcNow;
                });

            _hubConnection.On<IEnumerable<EmaCrossEntity>>(HubNotificationType.EMA_CROSS_COLLECTION,
                async entities =>
                {
                    _lastNotificationDT = DateTime.UtcNow;
                    await _cacheService.EmaCross.UpdateAsync(entities);
                });

            await _hubConnection.StartAsync();
        }

        public async Task CheckConnection()
        {
            var isTimeoutReached = (DateTime.UtcNow - _lastNotificationDT).TotalMilliseconds > TIMEOUT_MS;
            if (!isTimeoutReached)
                return;

            var isTimeToReconnect = (DateTime.UtcNow - _lastReconnectAttempt).TotalMilliseconds > TIMEOUT_MS;
            if (!isTimeToReconnect)
                return;

            if (StateChanged != null)
                await StateChanged.Invoke();

            await StartAsync();

            _lastReconnectAttempt = DateTime.UtcNow;
        }

        private async Task hubConnection_Closed(Exception? arg)
        {
            if (StateChanged != null)
                await StateChanged.Invoke();
        }

        private async Task hubConnection_Reconnecting(Exception? arg)
        {
            if (StateChanged != null)
                await StateChanged.Invoke();
        }

        private async Task hubConnection_Reconnected(string? arg)
        {
            if (StateChanged != null)
                await StateChanged.Invoke();
        }
    }
}