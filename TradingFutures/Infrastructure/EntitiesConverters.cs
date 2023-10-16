using Huobi.Net.Enums;
using Huobi.Net.Objects.Models.UsdtMarginSwap;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Infrastructure
{
    public static class EntitiesConverters
    {
        public static TradingOrderSide ToTradingOrderSide(OrderSide orderSide)
        {
            switch (orderSide)
            {
                case OrderSide.Buy: return TradingOrderSide.BUY;
                case OrderSide.Sell: return TradingOrderSide.SELL;
                default: return TradingOrderSide._EMPTY;
            }
        }

        public static TradingOrderOffset ToTradingOrderOffset(Offset offset)
        {
            switch (offset)
            {
                case Offset.Open: return TradingOrderOffset.OPEN;
                case Offset.Close: return TradingOrderOffset.CLOSE;
                case Offset.Both: return TradingOrderOffset.BOTH;
                default: return TradingOrderOffset._EMPTY;
            }
        }

        public static TradingMarginMode ToTradingMarginMode(MarginMode marginMode)
        {
            switch (marginMode)
            {
                case MarginMode.Cross: return TradingMarginMode.CROSS;
                case MarginMode.Isolated: return TradingMarginMode.ISOLATED;
                case MarginMode.All: return TradingMarginMode.ALL;
                default: return TradingMarginMode._EMPTY;
            }
        }

        public static TradingPositionMode ToTradingPositionMode(PositionMode positionMode)
        {
            switch (positionMode)
            {
                case PositionMode.SingleSide: return TradingPositionMode.SINGLE_SIDE;
                case PositionMode.DualSide: return TradingPositionMode.DUAL_SIDE;
                default: return TradingPositionMode._EMPTY;
            }
        }

        public static TradingOrderStatus ToTradingOrderStatus(SwapMarginOrderStatus status)
        {
            switch (status)
            {
                case SwapMarginOrderStatus.ReadyToSubmit: return TradingOrderStatus.READY_TO_SUBMIT;
                case SwapMarginOrderStatus.Submitting: return TradingOrderStatus.SUBMITTING;
                case SwapMarginOrderStatus.Submitted: return TradingOrderStatus.SUBMITTED;
                case SwapMarginOrderStatus.PartiallyFilled: return TradingOrderStatus.PARTIALLY_FILLED;
                case SwapMarginOrderStatus.PartiallyCancelled: return TradingOrderStatus.PARTIALLY_CANCELLED;
                case SwapMarginOrderStatus.Filled: return TradingOrderStatus.FILLED;
                case SwapMarginOrderStatus.Cancelled: return TradingOrderStatus.CANCELLED;
                case SwapMarginOrderStatus.Cancelling: return TradingOrderStatus.CANCELLING;

                default: return TradingOrderStatus._EMPTY;
            }
        }

        public static TradingPositionEntity ToTradingPositionEntity(HuobiPosition huobiPosition)
        {
            if (huobiPosition == null)
                throw new ArgumentNullException(nameof(huobiPosition));

            return new TradingPositionEntity
            {
                Asset = huobiPosition.Asset,
                ContractCode = huobiPosition.ContractCode,
                Volume = huobiPosition.Volume,
                Available = huobiPosition.Available,
                Frozen = huobiPosition.Frozen,
                CostOpen = huobiPosition.CostOpen,
                CostHold = huobiPosition.CostHold,
                ProfitUnreal = huobiPosition.ProfitUnreal,
                ProfitRate = huobiPosition.ProfitRate,
                LeverRate = huobiPosition.LeverRate,
                PositionMargin = huobiPosition.PositionMargin,
                OrderSide = ToTradingOrderSide(huobiPosition.Side),
                Profit = huobiPosition.Profit,
                LastPrice = huobiPosition.LastPrice,
                MarginAsset = huobiPosition.MarginAsset,
                MarginMode = ToTradingMarginMode(huobiPosition.MarginMode),
                MarginAccount = huobiPosition.MarginAccount,
                PositionMode = ToTradingPositionMode(huobiPosition.PositionMode)
            };
        }

        public static TradingTransactionEntity ToTradingTransactionEntity(HuobiMarginOrderDetails huobiOrderDetails)
        {
            return new TradingTransactionEntity
            {
                OrderDT = huobiOrderDetails.CreateTime,
                Asset = huobiOrderDetails.Asset,
                OrderSide = ToTradingOrderSide(huobiOrderDetails.Side),
                ContractCode = huobiOrderDetails.ContractCode,
                OrderOffset = ToTradingOrderOffset(huobiOrderDetails.Offset),
                OrderStatus = ToTradingOrderStatus(huobiOrderDetails.Status),
                Volume = huobiOrderDetails.Quantity,
                TradeVolume = huobiOrderDetails.QuantityFilled,
                TradeTurnover = huobiOrderDetails.ValueFilled,
                Price = huobiOrderDetails.Price,
                TradeAvgPrice = huobiOrderDetails.AverageFillPrice,
                Fee = huobiOrderDetails.Fee,
                FeeAsset = huobiOrderDetails.FeeAsset,
                Profit = huobiOrderDetails.Profit,
                RealProfit = huobiOrderDetails.RealProfit,
                LeverageRate = huobiOrderDetails.LeverageRate,
                OrderId = huobiOrderDetails.OrderId
            };
        }
    }
}