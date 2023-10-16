using TradingFutures.Shared.Entities;

namespace TradingFutures.Infrastructure.TradingApiClientServices.DTO
{
    public class EmaCrossDTO
    {
        public DateTime ReferenceDT { get; set; }

        public string CryptoAsset { get; set; } = string.Empty;

        public string TAInterval { get; set; } = string.Empty;

        public decimal ValueShort { get; set; }

        public decimal ValueLong { get; set; }

        public decimal PrevValueShort { get; set; }

        public decimal PrevValueLong { get; set; }

        public bool? SlopeShortTrendUp { get; set; }

        public bool? SlopeLongTrendUp { get; set; }

        public EmaCrossEntity ToEntity()
        {
            return new EmaCrossEntity
            {
                ReferenceDT = ReferenceDT,
                Asset = CryptoAsset,
                Interval = TAInterval?.ToUpper() ?? string.Empty,
                ValueShort = ValueShort,
                ValueLong = ValueLong,
                PrevValueShort = PrevValueShort,
                PrevValueLong = PrevValueLong,
                SlopeShortTrendUp = SlopeShortTrendUp,
                SlopeLongTrendUp = SlopeLongTrendUp
            };
        }
    }
}