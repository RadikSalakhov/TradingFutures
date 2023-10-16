using TradingFutures.Shared.Utils;

namespace TradingFutures.Application.Configuration
{
    public class GeneralOptions
    {
        public bool IsProduction { get; set; }

        public string TradingApiUrl { get; set; } = string.Empty;
    }
}