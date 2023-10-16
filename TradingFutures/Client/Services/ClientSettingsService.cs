using System.Drawing;
using TradingFutures.Client.Abstraction;
using TradingFutures.Client.Services.ClientSettingsEntries;
using TradingFutures.Shared.Data;

namespace TradingFutures.Client.Services
{
    public class ClientSettingsService : IClientSettingsService
    {
        public event Func<Task>? Updated;

        public CurrentPageClientSettingsEntry CurrentPage { get; } = new CurrentPageClientSettingsEntry();

        public CurrentTradingProfileClientSettingsEntry CurrentTradingProfile { get; } = new CurrentTradingProfileClientSettingsEntry();

        public FreezeSortClientSettingsEntry FreezeSort { get; } = new FreezeSortClientSettingsEntry();

        public IsShortModeClientSettingsEntry IsShortMode { get; } = new IsShortModeClientSettingsEntry();

        public ManualModeEnabledClientSettingsEntry ManualModeEnabled { get; } = new ManualModeEnabledClientSettingsEntry();

        public ShowInfoClientSettingsEntry ShowInfo { get; } = new ShowInfoClientSettingsEntry();

        public SortDirectionClientSettingsEntry SortDirection { get; } = new SortDirectionClientSettingsEntry();

        public SortModeClientSettingsEntry SortMode { get; } = new SortModeClientSettingsEntry();

        public WindowSizeClientSettingsEntry WindowSize { get; } = new WindowSizeClientSettingsEntry();

        public ClientSettingsService()
        {
            CurrentPage.Updated += currentPage_updated;
            CurrentTradingProfile.Updated += currentTradingProfile_updated;
            FreezeSort.Updated += freezeSort_updated;
            IsShortMode.Updated += isShortMode_Updated;
            ManualModeEnabled.Updated += manualModeEnabled_Updated;
            ShowInfo.Updated += isShortMode_Updated;
            SortDirection.Updated += sortDirection_Updated;
            SortMode.Updated += sortMode_Updated;
            WindowSize.Updated += windowSize_Updated;
        }

        public async Task RaiseUpdated()
        {
            if (Updated != null)
                await Updated.Invoke();
        }

        private async Task currentPage_updated(int value)
        {
            await RaiseUpdated();
        }

        private async Task currentTradingProfile_updated(TradingProfileId value)
        {
            await RaiseUpdated();
        }

        private async Task freezeSort_updated(bool value)
        {
            await RaiseUpdated();
        }

        private async Task isShortMode_Updated(bool value)
        {
            await RaiseUpdated();
        }

        private async Task tradingConditionType_Updated(TradingConditionType value)
        {
            await RaiseUpdated();
        }

        private async Task manualModeEnabled_Updated(bool arg)
        {
            await RaiseUpdated();
        }

        private async Task sortDirection_Updated(SortDirection value)
        {
            await RaiseUpdated();
        }

        private async Task sortMode_Updated(SortMode value)
        {
            await RaiseUpdated();
        }

        private async Task windowSize_Updated(Size value)
        {
            await RaiseUpdated();
        }
    }
}