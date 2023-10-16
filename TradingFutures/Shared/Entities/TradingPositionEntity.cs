using System.Reflection.Metadata.Ecma335;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Keys;
using TradingFutures.Shared.Utils;

namespace TradingFutures.Shared.Entities
{
    public class TradingPositionEntity
    {
        public TradingOrderSide OrderSide { get; set; }

        public string Asset { get; set; } = string.Empty;

        public string ContractCode { get; set; } = string.Empty;

        public decimal Volume { get; set; }

        public decimal Available { get; set; }

        public decimal Frozen { get; set; }

        public decimal CostOpen { get; set; }

        public decimal CostHold { get; set; }

        public decimal ProfitUnreal { get; set; }

        public decimal ProfitRate { get; set; }

        public decimal LeverRate { get; set; }

        public decimal PositionMargin { get; set; }

        public decimal Profit { get; set; }

        public decimal LastPrice { get; set; }

        public string MarginAsset { get; set; } = string.Empty;

        public TradingMarginMode MarginMode { get; set; }

        public string MarginAccount { get; set; } = string.Empty;

        public TradingPositionMode PositionMode { get; set; }

        public decimal ContractSize { get; set; }

        public int VolumeStep { get; set; }

        public decimal OppositeOrderStepsAmount { get; set; }

        public DateTime LastCacheUpdateDT { get; set; } = DateTime.MinValue;

        public decimal? LastTransactionPrice { get; set; }

        public bool AutoBuyThresholdReached { get; set; }

        public TradingPositionSettingsEntity? TradingPositionSettings { get; set; }

        public bool IsLongPosition()
        {
            return OrderSide == TradingOrderSide.BUY;
        }

        public bool IsShortPosition()
        {
            return OrderSide == TradingOrderSide.SELL;
        }

        public bool HasCoins()
        {
            return Volume > 0m && ContractSize >= 0m;
        }

        public decimal GetCoins()
        {
            return Volume * ContractSize;
        }

        public decimal GetSpentUSDT()
        {
            return CostOpen * GetCoins();
        }

        public decimal GetCurrentUSDT()
        {
            return CostHold * GetCoins();
        }

        public decimal GetPnl()
        {
            return Profit;
            //return GetCurrentUSDT() - GetSpentUSDT();
        }

        public decimal GetPnlCoeff()
        {
            var spentUSDT = GetSpentUSDT();
            if (spentUSDT == 0m)
                return 0m;

            return GetPnl() / spentUSDT;

            //return ProfitRate;
        }

        public decimal GetCoinsStep()
        {
            return ContractSize * VolumeStep;
        }

        public decimal GetStepsAmount()
        {
            var coinsStep = GetCoinsStep();
            return coinsStep > 0m
                ? GetCoins() / coinsStep
                : 0m;
        }

        public int GetStepsAmountInt()
        {
            return (int)Math.Floor(GetStepsAmount());
        }

        public int GetOppositeStepsAmountInt()
        {
            return (int)Math.Floor(OppositeOrderStepsAmount);
        }

        public decimal GetSpentUSDTOverStepsAmount()
        {
            var stepsAmount = GetStepsAmount();

            return stepsAmount > 0m
                ? GetSpentUSDT() / stepsAmount
                : 0m;
        }

        public decimal GetPnlOverStepsAmount()
        {
            var stepsAmount = GetStepsAmount();

            return stepsAmount > 0m
                ? GetPnl() / stepsAmount
                : 0m;
        }

        public decimal GetPnlCoeffOverStepsAmount()
        {
            var pnlCoeff = GetPnlCoeff();
            var stepsAmount = GetStepsAmount();

            return stepsAmount > 0m
                ? pnlCoeff / stepsAmount
                : 0m;
        }

        public decimal GetLastTransactionPriceCoeff()
        {
            if (!LastTransactionPrice.HasValue || LastTransactionPrice.Value <= 0)
                return 0m;

            var diff = LastPrice - LastTransactionPrice.Value;
            var coeff = diff / LastTransactionPrice.Value;

            return coeff;
        }

        public bool IsPnlThreasholdReached(decimal minPnlCoeff)
        {
            return GetPnlOverStepsAmount() >= minPnlCoeff;
        }

        public decimal GetEmaShortPositionCoeff(EmaCrossEntity emaCross)
        {
            if (emaCross == null || emaCross.Asset != Asset)
                return 0M;

            if (!HasCoins())
                return 0M;

            if (IsLongPosition() && LastPrice > CostOpen)
                return MathUtilities.FitClamp(emaCross.ValueShort, CostOpen, LastPrice, 0, 1);

            if (IsShortPosition() && LastPrice < CostOpen)
                return MathUtilities.FitClamp(emaCross.ValueShort, CostOpen, LastPrice, 0, 1);

            return 0M;
        }

        public TradingPositionSettingsKey ToTradingPositionSettingsKey()
        {
            return new TradingPositionSettingsKey
            {
                Asset = Asset,
                OrderSide = OrderSide
            };
        }

        public static string GetContractCode(string asset)
        {
            return !string.IsNullOrWhiteSpace(asset)
                ? $"{asset}-USDT"
                : string.Empty;
        }

        public static TradingPositionEntity Create(TradingOrderSide tradingOrderSide, string asset)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException(GeneralConstants.CANT_BE_NULL_AND_EMPTY_MESSAGE, nameof(asset));

            return new TradingPositionEntity
            {
                OrderSide = tradingOrderSide,
                Asset = asset,
                ContractCode = GetContractCode(asset)
            };
        }
    }
}