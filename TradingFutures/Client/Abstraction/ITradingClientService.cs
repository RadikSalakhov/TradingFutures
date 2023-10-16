using TradingFutures.Shared.Entities;

namespace TradingFutures.Client.Abstraction
{
    public interface ITradingClientService
    {
        #region Trading

        Task<bool> UpdateServerSettings(ServerSettingsEntity serverSettings);

        Task<bool> UpdateTradingPositionSettings(TradingPositionSettingsEntity tradingPositionSettings);

        Task<bool> OpenLong(string asset);

        Task<bool> CloseLong(string asset);

        Task<bool> OpenShort(string asset);

        Task<bool> CloseShort(string asset);

        #endregion

        #region Trading Profiles

        Task<IEnumerable<TradingProfileEntity>> GetAllTradingProfiles(bool updateCache = false);

        Task<bool> UpdateTradingCondition(TradingConditionEntity tradingCondition);

        #endregion
    }
}