using TradingFutures.Client.Services.ClientSettingsEntries;

namespace TradingFutures.Client.Abstraction
{
    public interface IClientSettingsService
    {
        public event Func<Task>? Updated;

        CurrentPageClientSettingsEntry CurrentPage { get; }

        CurrentTradingProfileClientSettingsEntry CurrentTradingProfile { get; }

        FreezeSortClientSettingsEntry FreezeSort { get; }

        IsShortModeClientSettingsEntry IsShortMode { get; }

        ManualModeEnabledClientSettingsEntry ManualModeEnabled { get; }

        ShowInfoClientSettingsEntry ShowInfo { get; }

        SortDirectionClientSettingsEntry SortDirection { get; }

        SortModeClientSettingsEntry SortMode { get; }

        WindowSizeClientSettingsEntry WindowSize { get; }

        public Task RaiseUpdated();
    }
}