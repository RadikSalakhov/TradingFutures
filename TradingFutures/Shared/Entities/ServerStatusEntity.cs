namespace TradingFutures.Shared.Entities
{
    public class ServerStatusEntity
    {
        public bool IsProduction { get; set; }

        public DateTime ServerTime { get; set; } = DateTime.MinValue;

        public decimal CommitedDayPnl { get; set; }

        public decimal CommitedMonthPnl { get; set; }

        public decimal CommitedYearPnl { get; set; }
    }
}