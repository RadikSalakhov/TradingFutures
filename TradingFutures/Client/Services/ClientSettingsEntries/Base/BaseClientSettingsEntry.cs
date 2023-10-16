using System.Threading.Tasks;

namespace TradingFutures.Client.Services.ClientSettingsEntries.Base
{
    public abstract class BaseClientSettingsEntry<TValue>
        where TValue : new()
    {
        public event Func<TValue, Task>? Updated;

        public TValue Value { get; private set; } = new TValue();

        protected BaseClientSettingsEntry()
        {
        }

        protected BaseClientSettingsEntry(TValue value)
        {
            Value = value;
        }

        protected async Task RaiseUpdated(TValue value)
        {
            if (Updated != null)
                await Updated.Invoke(value);
        }

        public async Task Set(TValue value)
        {
            Value = value ?? new TValue();

            await RaiseUpdated(value);
        }
    }
}