using Microsoft.AspNetCore.SignalR.Client;

namespace TradingFutures.Client.Hubs
{
    public interface IHubClient : IAsyncDisposable
    {
        bool IsConnected { get; }

        HubConnectionState HubConnectionState { get; }

        event Func<Task>? StateChanged;

        Task StartAsync();

        Task CheckConnection();
    }
}