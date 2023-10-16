using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Configuration;
using TradingFutures.Infrastructure.TradingApiClientServices.DTO;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Infrastructure.TradingApiClientServices
{
    public class TradingApiClientService : ITradingApiClientService
    {
        private readonly GeneralOptions _generalOptions;

        public TradingApiClientService(IOptions<GeneralOptions> generalOptions)
        {
            _generalOptions = generalOptions.Value;
        }

        public async Task<IEnumerable<EmaCrossEntity>> GetEmaCross(string interval)
        {
            if (string.IsNullOrWhiteSpace(_generalOptions.TradingApiUrl))
                return Array.Empty<EmaCrossEntity>();

            using HttpClient httpClient = new HttpClient { BaseAddress = new Uri(_generalOptions.TradingApiUrl) };

            var dtos = await httpClient.GetFromJsonAsync<IEnumerable<EmaCrossDTO>>($"api/ta/get-ema-cross?interval={interval}");

            return dtos != null
                ? dtos.Select(v => v.ToEntity())
                : Array.Empty<EmaCrossEntity>();
        }
    }
}