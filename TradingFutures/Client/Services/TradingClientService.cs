using System.Net;
using System.Net.Http.Json;
using TradingFutures.Client.Abstraction;
using TradingFutures.Shared.Abstraction;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Client.Services
{
    public class TradingClientService : ITradingClientService
    {
        private const string TRADING = "trading";
        private const string TRADING_PROFILES = "trading-profiles";

        private readonly HttpClient _httpClient;

        private readonly ICacheService _cacheService;

        public static Func<string>? PasswordFunc { get; set; }

        public string Password
        {
            get
            {
                return PasswordFunc != null ? PasswordFunc() : string.Empty;
            }
        }

        public TradingClientService(HttpClient httpClient, ICacheService cacheService)
        {
            _httpClient = httpClient;
            _cacheService = cacheService;
        }

        #region Trading

        public async Task<bool> UpdateServerSettings(ServerSettingsEntity serverSettings)
        {
            if (serverSettings == null)
                return false;

            var result = await _httpClient.PostAsJsonAsync<ServerSettingsEntity>($"api/{TRADING}/update-server-settings?pwd={Password}", serverSettings);

            return result?.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> UpdateTradingPositionSettings(TradingPositionSettingsEntity tradingPositionSettings)
        {
            if (tradingPositionSettings == null)
                return false;

            var result = await _httpClient.PostAsJsonAsync<TradingPositionSettingsEntity>($"api/{TRADING}/update-trading-position-settings?pwd={Password}", tradingPositionSettings);

            return result?.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> OpenLong(string asset)
        {
            var result = await _httpClient.GetAsync($"api/{TRADING}/open-long?asset={asset}&pwd={Password}");

            return result?.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> CloseLong(string asset)
        {
            var result = await _httpClient.GetAsync($"api/{TRADING}/close-long?asset={asset}&pwd={Password}");

            return result?.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> OpenShort(string asset)
        {
            var result = await _httpClient.GetAsync($"api/{TRADING}/open-short?asset={asset}&pwd={Password}");

            return result?.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> CloseShort(string asset)
        {
            var result = await _httpClient.GetAsync($"api/{TRADING}/close-short?asset={asset}&pwd={Password}");

            return result?.StatusCode == HttpStatusCode.OK;
        }

        #endregion

        #region Trading Profiles

        public async Task<IEnumerable<TradingProfileEntity>> GetAllTradingProfiles(bool updateCache)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<TradingProfileEntity>>($"api/{TRADING_PROFILES}/get-all?pwd={Password}");
            if (result == null)
                result = Array.Empty<TradingProfileEntity>();

            if (updateCache)
                await _cacheService.TradingProfile.UpdateAsync(result, clear: true);

            return result;
        }

        public async Task<bool> UpdateTradingCondition(TradingConditionEntity tradingCondition)
        {
            if (tradingCondition == null)
                return false;

            var result = await _httpClient.PostAsJsonAsync<TradingConditionEntity>($"api/{TRADING_PROFILES}/update-trading-condition?pwd={Password}", tradingCondition);

            return result?.StatusCode == HttpStatusCode.OK;
        }

        #endregion
    }
}