using TradingFutures.Client.Services.ClientSettingsEntries.Base;
using TradingFutures.Shared.Data;

namespace TradingFutures.Client.Services.ClientSettingsEntries
{
    public class SortDirectionClientSettingsEntry : BaseClientSettingsEntry<SortDirection>
    {
        public SortDirectionClientSettingsEntry()
            : base(SortDirection.ASC)
        {
        }

        public async Task ToggleSortDirection()
        {
            if (Value == SortDirection._EMPTY)
                await Set(SortDirection.ASC);
            else if (Value == SortDirection.ASC)
                await Set(SortDirection.DESC);
            else
                await Set(SortDirection._EMPTY);
        }
    }
}