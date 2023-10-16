using TradingFutures.Client.Services.ClientSettingsEntries.Base;
using TradingFutures.Shared.Data;

namespace TradingFutures.Client.Services.ClientSettingsEntries
{
    public class CurrentTradingProfileClientSettingsEntry : BaseClientSettingsEntry<TradingProfileId>
    {
        public CurrentTradingProfileClientSettingsEntry()
            : base(TradingProfileId.ASSISTANT)
        {
        }
    }
}