namespace TradingFutures.Shared.Services.CacheEntries.Base
{
    public abstract class BaseCacheEntry<TValue>
    {
        public event Func<IEnumerable<TValue>, Task>? Updated;

        protected virtual Task OnUpdated(IEnumerable<TValue> values)
        {
            return Task.CompletedTask;
        }

        protected async Task RaiseUpdated(IEnumerable<TValue> values)
        {
            var updatedFunc = Updated;

            await OnUpdated(values ?? Array.Empty<TValue>());

            if (updatedFunc != null)
                await updatedFunc.Invoke(values ?? Array.Empty<TValue>());
        }
    }
}