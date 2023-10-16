using TradingFutures.Shared.Base;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Shared.Entities
{
    public class TradingTransactionEntity : BaseEntity<TradingTransactionKey, TradingTransactionEntity>
    {
        public DateTime OrderDT
        {
            get => Key.OrderDT;
            set => Key.OrderDT = value;
        }

        public string Asset
        {
            get => Key.Asset;
            set => Key.Asset = value;
        }

        public TradingOrderSide OrderSide
        {
            get => Key.OrderSide;
            set => Key.OrderSide = value;
        }

        public string ContractCode { get; set; } = string.Empty;

        public TradingOrderOffset OrderOffset { get; set; }

        public TradingOrderStatus OrderStatus { get; set; }

        public decimal Volume { get; set; }

        public decimal TradeVolume { get; set; }

        public decimal TradeTurnover { get; set; }

        public decimal Price { get; set; }

        public decimal? TradeAvgPrice { get; set; }

        public decimal Fee { get; set; }

        public string FeeAsset { get; set; } = string.Empty;

        public decimal Profit { get; set; }

        public decimal RealProfit { get; set; }

        public int LeverageRate { get; set; }

        public long OrderId { get; set; }

        public string Label { get; set; }

        public TradingTransactionEntity()
        {
        }

        public TradingTransactionEntity(TradingTransactionKey key)
            : base(key)
        {
        }

        protected override TradingTransactionEntity CreateInstanceInternal(TradingTransactionKey key)
        {
            return new TradingTransactionEntity(key);
        }

        protected override void CopyFromInternal(TradingTransactionEntity entity)
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

        public TradingOrderType GetTradingOrderType()
        {
            return TradingOrderType.ToTradingOrderType(OrderSide, OrderOffset);
        }

        public decimal GetPNL()
        {
            return RealProfit + 2 * Fee;
        }
    }
}