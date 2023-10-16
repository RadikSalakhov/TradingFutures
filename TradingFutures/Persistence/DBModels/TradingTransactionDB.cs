using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingFutures.Persistence.Base;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.DBModels
{
    [Table("TradingTransaction")]
    [PrimaryKey("OrderDT", "Asset", "OrderSide")]
    public class TradingTransactionDB : BaseDB<TradingTransactionKey, TradingTransactionEntity, TradingTransactionDB>
    {
        public DateTime OrderDT { get; set; }

        [StringLength(20)]
        public string Asset { get; set; } = string.Empty;

        [StringLength(10)]
        public string OrderSide { get; set; } = string.Empty;

        [StringLength(40)]
        public string ContractCode { get; set; } = string.Empty;

        [StringLength(10)]
        public string OrderOffset { get; set; } = string.Empty;

        [StringLength(30)]
        public string OrderStatus { get; set; } = string.Empty;

        [Column(TypeName = "decimal(38,19)")]
        public decimal Volume { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal TradeVolume { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal TradeTurnover { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal? TradeAvgPrice { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal Fee { get; set; }

        public string FeeAsset { get; set; } = string.Empty;

        [Column(TypeName = "decimal(38,19)")]
        public decimal Profit { get; set; }

        [Column(TypeName = "decimal(38,19)")]
        public decimal RealProfit { get; set; }

        public int LeverageRate { get; set; }

        public long OrderId { get; set; }

        protected override void FromKeyInternal(TradingTransactionKey key)
        {
            OrderDT = key.OrderDT;
            Asset = key.Asset;
            OrderSide = key.OrderSide;
        }

        protected override TradingTransactionKey ToKeyInternal()
        {
            return new TradingTransactionKey(OrderDT, Asset, (TradingOrderSide)OrderSide);
        }

        protected override void FromEntityInternal(TradingTransactionEntity entity)
        {
            ContractCode = entity.ContractCode;
            OrderOffset = entity.OrderOffset;
            OrderStatus = entity.OrderStatus;
            Volume = entity.Volume;
            TradeVolume = entity.TradeVolume;
            TradeTurnover = entity.TradeTurnover;
            Price = entity.Price;
            TradeAvgPrice = entity.TradeAvgPrice;
            Fee = entity.Fee;
            FeeAsset = entity.FeeAsset;
            Profit = entity.Profit;
            RealProfit = entity.RealProfit;
            LeverageRate = entity.LeverageRate;
            OrderId = entity.OrderId;
        }

        protected override TradingTransactionEntity ToEntityInternal()
        {
            return new TradingTransactionEntity(ToKey())
            {
                ContractCode = ContractCode,
                OrderOffset = (TradingOrderOffset)OrderOffset,
                OrderStatus = (TradingOrderStatus)OrderStatus,
                Volume = Volume,
                TradeVolume = TradeVolume,
                TradeTurnover = TradeTurnover,
                Price = Price,
                TradeAvgPrice = TradeAvgPrice,
                Fee = Fee,
                FeeAsset = FeeAsset,
                Profit = Profit,
                RealProfit = RealProfit,
                LeverageRate = LeverageRate,
                OrderId = OrderId
            };
        }
    }
}