namespace TradingFutures.Shared.Base
{
    public abstract class BaseKey<T> : IEquatable<T>
        where T : BaseKey<T>
    {
        public abstract bool Equals(T? other);

        public abstract bool IsValid();

        public abstract T GetCopy();
    }
}