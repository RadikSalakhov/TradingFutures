﻿using TradingFutures.Application.Abstraction.RepositoryServices.Base;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Application.Abstraction.RepositoryServices
{
    public interface ITradingConditionRepositoryService : IBaseRepositoryService<TradingConditionKey, TradingConditionEntity>
    {
    }
}