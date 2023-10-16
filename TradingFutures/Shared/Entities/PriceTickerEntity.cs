namespace TradingFutures.Shared.Entities
{
    public class PriceTickerEntity
    {
        public DateTime ReferenceDT { get; set; }

        public string Asset { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}