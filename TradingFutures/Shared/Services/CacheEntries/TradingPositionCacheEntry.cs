using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Services.CacheEntries.Base;

namespace TradingFutures.Shared.Services.CacheEntries
{
    public class TradingPositionCacheEntry : BaseDoubleDictCacheEntry<TradingOrderSide, string, TradingPositionEntity>
    {
        protected override TradingOrderSide GetKeyA(TradingPositionEntity value)
        {
            return value.OrderSide;
        }

        protected override string GetKeyB(TradingPositionEntity value)
        {
            return value.Asset;
        }

        protected override async Task OnUpdated(IEnumerable<TradingPositionEntity> values)
        {
            await base.OnUpdated(values);

            var utcNow = DateTime.UtcNow;
            foreach (var value in values)
                value.LastCacheUpdateDT = utcNow;
        }

        public decimal GetAverageStepUSDT()
        {
            var stepsValues = new List<decimal>();
            foreach (var tradingPosition in GetValues())
            {
                var stepUSDT = tradingPosition.GetSpentUSDTOverStepsAmount();
                if (stepUSDT > 0)
                    stepsValues.Add(stepUSDT);
            }

            return stepsValues.Any()
                ? stepsValues.Average()
                : 0m;
        }

        public IEnumerable<TradingPositionEntity> GetSortedPositions(TradingOrderSide tradingOrderSide, SortMode sortMode, SortDirection sortDirection)
        {
            var sourceValues = GetValues(new[] { tradingOrderSide });

            if (sortDirection == SortDirection._EMPTY)
                return sourceValues.OrderBy(v => v.Asset);

            Func<TradingPositionEntity, decimal>? sortBy = null;

            if (sortMode == SortMode.PNL)
                sortBy = (tradingPosition) => tradingPosition.GetPnl();
            else if (sortMode == SortMode.PNL_NORMALIZED)
            {
                sortBy = (tradingPosition) => tradingPosition.GetPnlOverStepsAmount();
            }

            if (sortBy != null)
            {
                if (sortDirection == SortDirection.ASC)
                    sourceValues = sourceValues.OrderBy(sortBy);
                else if (sortDirection == SortDirection.DESC)
                    sourceValues = sourceValues.OrderByDescending(sortBy);
            }
            else
            {
                if (sortDirection == SortDirection.ASC)
                    sourceValues = sourceValues.OrderBy(v => v.Asset);
                else if (sortDirection == SortDirection.DESC)
                    sourceValues = sourceValues.OrderByDescending(v => v.Asset);
            }

            return sourceValues;
        }

        public TradingPositionEntity GetTradingPositionOrDefault(TradingOrderSide tradingOrderSide, string asset)
        {
            return GetByKeys(tradingOrderSide, asset) ?? TradingPositionEntity.Create(tradingOrderSide, asset);
        }

        public TradingPositionEntity GetOppositeTradingPositionOrDefault(TradingOrderSide tradingOrderSide, string asset)
        {
            if (tradingOrderSide == TradingOrderSide.BUY)
                tradingOrderSide = TradingOrderSide.SELL;
            else if (tradingOrderSide == TradingOrderSide.SELL)
                tradingOrderSide = TradingOrderSide.BUY;

            return GetByKeys(tradingOrderSide, asset) ?? TradingPositionEntity.Create(tradingOrderSide, asset);
        }

        public decimal GetTotalPnlLong()
        {
            return GetValues(new[] { TradingOrderSide.BUY }).Sum(v => v.GetPnl());
        }

        public decimal GetTotalPnlShort()
        {
            return GetValues(new[] { TradingOrderSide.SELL }).Sum(v => v.GetPnl());
        }

        public decimal GetCurrentUSDTLong()
        {
            return GetValues(new[] { TradingOrderSide.BUY }).Sum(v => v.GetCurrentUSDT());
        }

        public decimal GetCurrentUSDTShort()
        {
            return GetValues(new[] { TradingOrderSide.SELL }).Sum(v => v.GetCurrentUSDT());
        }
    }
}