using TradingFutures.Shared.Base;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Shared.Entities
{
    public class TradingProfileEntity : BaseEntity<TradingProfileKey, TradingProfileEntity>
    {
        public string TradingProfileId
        {
            get => Key.TradingProfileId;
            set => Key.TradingProfileId = value;
        }

        public IList<TradingConditionEntity> TradingConditions { get; set; } = new List<TradingConditionEntity>();

        public TradingProfileEntity()
        {
        }

        public TradingProfileEntity(TradingProfileKey key)
            : base(key)
        {
        }

        protected override TradingProfileEntity CreateInstanceInternal(TradingProfileKey key)
        {
            return new TradingProfileEntity(key);
        }

        protected override void CopyFromInternal(TradingProfileEntity entity)
        {
            SetTradingConditions(entity.TradingConditions);
        }

        public void SetTradingConditions(IEnumerable<TradingConditionEntity> tradingConditions)
        {
            lock (TradingConditions)
            {
                TradingConditions.Clear();
                foreach (var tradingCondition in tradingConditions)
                    TradingConditions.Add(tradingCondition.GetCopy());
            }
        }
    }
}