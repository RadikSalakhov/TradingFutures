using TradingFutures.Client.Services.ClientSettingsEntries.Base;

namespace TradingFutures.Client.Services.ClientSettingsEntries
{
    public class ShowInfoClientSettingsEntry : BaseClientSettingsEntry<bool>
    {
        public async Task Toggle()
        {
            await Set(!Value);
        }
    }
}