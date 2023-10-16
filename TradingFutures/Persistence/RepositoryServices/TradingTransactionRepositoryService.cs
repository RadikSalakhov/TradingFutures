using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Application.Configuration;
using TradingFutures.Persistence.Base;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Persistence.DBModels;
using TradingFutures.Shared.Data;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.RepositoryServices
{
    public class TradingTransactionRepositoryService : BaseRepositoryService<TradingTransactionKey, TradingTransactionEntity, TradingTransactionDB>, ITradingTransactionRepositoryService
    {
        private readonly string[] _assets;

        public TradingTransactionRepositoryService(DataContext dataContext, IConfiguration configuration)
            : base(dataContext, dataContext.TradingTransactions)
        {
            _assets = ContractOptions.ParseFromConfiguration(configuration).Values.Select(v => v.Asset).ToArray();
        }

        protected override IQueryable<TradingTransactionDB> ApplyFilterByKey(IQueryable<TradingTransactionDB> query, TradingTransactionKey key)
        {
            return query.Where(v => v.OrderDT == key.OrderDT && v.Asset == key.Asset && v.OrderSide == key.OrderSide);
        }

        public async Task<IEnumerable<TradingTransactionEntity>> GetAll(string? asset, TradingOrderSide? orderSide)
        {
            var query = GetDefaultQuery();

            if (!string.IsNullOrWhiteSpace(asset))
                query = query.Where(v => v.Asset == asset);

            if (orderSide.HasValue)
                query = query.Where(v => v.OrderSide == orderSide.Value);

            var records = await query.ToListAsync();

            return records.Select(v => v.ToEntity()).ToList();
        }

        public async Task<IEnumerable<TradingTransactionEntity>> GetByDates(DateTime minDTIncl, DateTime maxDTExcl, int? takeAmount)
        {
            var query = GetDefaultQuery()
                .Where(v => v.OrderDT >= minDTIncl && v.OrderDT < maxDTExcl);

            if (takeAmount.HasValue)
                query = query.OrderByDescending(v => v.OrderDT).Take(takeAmount.Value);

            var records = await query.ToListAsync();

            return records.Select(v => v.ToEntity()).ToList();
        }

        public async Task<IEnumerable<TradingTransactionEntity>> GetLast(int takeAmount)
        {
            var query = GetDefaultQuery()
                .OrderByDescending(v => v.OrderDT)
                .Take(takeAmount);

            var records = await query.ToListAsync();

            return records.Select(v => v.ToEntity()).ToList();
        }

        public async Task<Dictionary<string, Dictionary<TradingOrderSide, TradingTransactionEntity>>> GetLastTransactions()
        {
            var query = GetDefaultQuery();

            query = query
                .GroupBy(v => new { v.Asset, v.OrderSide })
                .Select(g => g.OrderByDescending(v => v.OrderDT).First());

            var records = await query.ToListAsync();
            var entities = records.Select(v => v.ToEntity()).ToList();

            return entities
                .GroupBy(v => v.Asset).ToDictionary(gA => gA.Key,
                    gA => gA.GroupBy(v => v.OrderSide).ToDictionary(gB => gB.Key, gB => gB.First()));
        }

        public async Task<IEnumerable<TradingTransactionEntity>> GetLastTransactions(IEnumerable<TradingPositionSettingsKey> keys)
        {
            if (keys == null || !keys.Any())
                return Array.Empty<TradingTransactionEntity>();

            var strKeys = keys.Select(v => $"{v.Asset}-{v.OrderSide}").ToArray();

            var query = GetDefaultQuery();

            query = query
                .GroupBy(v => new { v.Asset, v.OrderSide })
                .Where(g => strKeys.Contains(string.Concat(g.Key.Asset, "-", g.Key.OrderSide)))
                .Select(g => g.OrderByDescending(v => v.OrderDT).First());

            var records = await query.ToListAsync();

            return records.Select(v => v.ToEntity()).ToList();
        }

        public async Task<decimal> GetPnlSum(DateTime minDTIncl, DateTime maxDTExcl)
        {
            var query = GetDefaultQuery()
                .Where(v => v.OrderDT >= minDTIncl && v.OrderDT < maxDTExcl);

            return await query.SumAsync(v => v.RealProfit + 2 * v.Fee);
        }

        public async Task<IEnumerable<TradingPnlEntity>> GetPnlData(DateTime minDTIncl, DateTime maxDTExcl)
        {
            var sourceQuery = GetDefaultQuery()
                .Where(v => v.OrderDT >= minDTIncl && v.OrderDT < maxDTExcl)
                .Select(sub => new
                {
                    sub.Asset,
                    Pnl = sub.RealProfit + 2 * sub.Fee
                });

            var sourceQueryNegative = sourceQuery.Where(v => v.Pnl < 0);
            var sourceQueryPositive = sourceQuery.Where(v => v.Pnl > 0);

            var targetQueryNegative = sourceQueryNegative
                .GroupBy(v => v.Asset)
                .Select(sub => new
                {
                    Asset = sub.Key,
                    Sum = sub.Sum(v => v.Pnl)
                });

            var targetQueryPositive = sourceQueryPositive
                .GroupBy(v => v.Asset)
                .Select(sub => new
                {
                    Asset = sub.Key,
                    Sum = sub.Sum(v => v.Pnl)
                });

            var negativeRecords = await targetQueryNegative.ToListAsync();
            var positiveRecords = await targetQueryPositive.ToListAsync();

            var resultList = new List<TradingPnlEntity>();

            foreach (var asset in _assets)
            {
                var spotTradingPnl = new TradingPnlEntity();
                spotTradingPnl.Asset = asset;
                spotTradingPnl.SumNegative = negativeRecords.Where(v => v.Asset == asset).FirstOrDefault()?.Sum ?? 0m;
                spotTradingPnl.SumPositive = positiveRecords.Where(v => v.Asset == asset).FirstOrDefault()?.Sum ?? 0m;

                resultList.Add(spotTradingPnl);
            }

            return resultList;
        }
    }
}