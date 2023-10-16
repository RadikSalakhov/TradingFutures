namespace TradingFutures.Shared.Data
{
    public struct HubNotificationType
    {
        public string Value { get; set; }

        public HubNotificationType(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static bool operator ==(HubNotificationType valueA, HubNotificationType valueB) => valueA.Value == valueB.Value;

        public static bool operator !=(HubNotificationType valueA, HubNotificationType valueB) => valueA.Value != valueB.Value;

        public static implicit operator string(HubNotificationType value) => value.Value;

        public static explicit operator HubNotificationType(string value) => new(value);

        public static HubNotificationType _EMPTY { get => new(string.Empty); }

        public static HubNotificationType SERVER_SETTINGS { get => new("SERVER_SETTINGS"); }

        public static HubNotificationType SERVER_STATUS { get => new("SERVER_STATUS"); }

        public static HubNotificationType TRADING_POSITIONS_COLLECTION { get => new("TRADING_POSITIONS_COLLECTION"); }

        public static HubNotificationType EMA_CROSS_COLLECTION { get => new("EMA_CROSS_COLLECTION"); }

        public override bool Equals(object? obj)
        {
            if (obj is not HubNotificationType)
                return false;

            return ((HubNotificationType)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}