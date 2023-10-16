using TradingFutures.Shared.Base;

namespace TradingFutures.Shared.Keys
{
    public class ServerSettingsKey : BaseKey<ServerSettingsKey>
    {
        public ServerSettingsKey()
        {
        }

        public override bool Equals(ServerSettingsKey? other)
        {
            return ReferenceEquals(this, other);
        }

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        int hash = 17;
        //        hash = hash * 23 + TypeName?.GetHashCode() ?? 0;
        //        hash = hash * 23 + PropertyName?.GetHashCode() ?? 0;
        //        return hash;
        //    }
        //}

        public override bool IsValid()
        {
            return true;
        }

        public override ServerSettingsKey GetCopy()
        {
            return new ServerSettingsKey();
        }
    }
}