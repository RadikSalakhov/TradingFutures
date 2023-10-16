using TradingFutures.Client.Services.ClientSettingsEntries.Base;

namespace TradingFutures.Client.Services.ClientSettingsEntries
{
    public class ManualModeEnabledClientSettingsEntry : BaseClientSettingsEntry<bool>
    {
        public async Task Toggle()
        {
            await Set(!Value);
        }
    }
}