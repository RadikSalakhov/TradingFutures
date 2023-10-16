using TradingFutures.Client.Services.ClientSettingsEntries.Base;
using TradingFutures.Shared.Data;

namespace TradingFutures.Client.Services.ClientSettingsEntries
{
    public class SortModeClientSettingsEntry : BaseClientSettingsEntry<SortMode>
    {
        public SortModeClientSettingsEntry()
            : base(SortMode.PNL_NORMALIZED)
        {
        }

        public async Task ToggleSortMode()
        {
            if (Value == SortMode._EMPTY)
                await Set(SortMode.PNL);
            else if (Value == SortMode.PNL)
                await Set(SortMode.PNL_NORMALIZED);
            else
                await Set(SortMode.PNL);
        }
    }
}