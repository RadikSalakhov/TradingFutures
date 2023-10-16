using TradingFutures.Client.Services.ClientSettingsEntries.Base;
using TradingFutures.Shared.Data;

namespace TradingFutures.Client.Services.ClientSettingsEntries
{
    public class IsShortModeClientSettingsEntry : BaseClientSettingsEntry<bool>
    {
        public async Task Toggle()
        {
            await Set(!Value);
        }

        public TradingOrderSide GetTradingOrderSide()
        {
            return Value ? TradingOrderSide.SELL : TradingOrderSide.BUY;
        }
    }
}