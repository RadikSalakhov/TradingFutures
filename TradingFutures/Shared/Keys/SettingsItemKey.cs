using TradingFutures.Shared.Base;

namespace TradingFutures.Shared.Keys
{
    public class SettingsItemKey : BaseKey<SettingsItemKey>
    {
        public string TypeName { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public SettingsItemKey()
        {
        }

        public SettingsItemKey(string typeName, string propertyName)
        {
            TypeName = typeName;
            PropertyName = propertyName;
        }

        public override bool Equals(SettingsItemKey? other)
        {
            return other != null &&
                other.TypeName == TypeName &&
                other.PropertyName == PropertyName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + TypeName?.GetHashCode() ?? 0;
                hash = hash * 23 + PropertyName?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(TypeName)
                && !string.IsNullOrWhiteSpace(PropertyName);
        }

        public override SettingsItemKey GetCopy()
        {
            return new SettingsItemKey(TypeName, PropertyName);
        }
    }
}