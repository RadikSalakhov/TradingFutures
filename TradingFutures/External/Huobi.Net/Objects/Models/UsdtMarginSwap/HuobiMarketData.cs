using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Huobi.Net.Objects.Models.UsdtMarginSwap
{
    /// <summary>
    /// Market data
    /// </summary>
    public class HuobiMarketData : HuobiSymbolData
    {
        /// <summary>
        /// Best ask
        /// </summary>
        [JsonConverter(typeof(ArrayConverter))]
        public HuobiOrderBookEntry? Ask { get; set; }

        /// <summary>
        /// Best bid
        /// </summary>
        [JsonConverter(typeof(ArrayConverter))]
        public HuobiOrderBookEntry? Bid { get; set; }
    }

    public class HuobiMarketDataEx : HuobiMarketData
    {
        [JsonProperty("contract_code")]
        public string ContractCode { get; set; }

        [JsonProperty("business_type")]
        public string BusinessType { get; set; }
    }
}