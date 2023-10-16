namespace TradingFutures.Shared.Entities
{
    public class TradingPnlEntity
    {
        public string Asset { get; set; } = string.Empty;

        public decimal SumNegative { get; set; }

        public decimal SumPositive { get; set; }

        public decimal SumTotal => SumNegative + SumPositive;
    }
}