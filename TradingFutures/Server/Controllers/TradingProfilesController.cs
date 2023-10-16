using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Application.Configuration;
using TradingFutures.Server.Controllers.Base;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Server.Controllers
{
    [Route("api/trading-profiles")]
    [ApiController]
    public class TradingProfilesController : BaseWebApiController
    {
        private readonly ITradingConditionRepositoryService _tradingConditionRepositoryService;
        private readonly ITradingProfileRepositoryService _tradingProfileRepositoryService;

        public TradingProfilesController(IOptions<DevOptions> devOptions,
            ITradingConditionRepositoryService tradingConditionRepositoryService,
            ITradingProfileRepositoryService tradingProfileRepositoryService)
            : base(devOptions)
        {
            _tradingConditionRepositoryService = tradingConditionRepositoryService;
            _tradingProfileRepositoryService = tradingProfileRepositoryService;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<TradingProfileEntity>>> GetAllProfiles()
        {
            var result = await _tradingProfileRepositoryService.GetAll() ?? Array.Empty<TradingProfileEntity>();

            return Ok(result);
        }

        [HttpPost("update-trading-condition")]
        public async Task<ActionResult> UpdateServerSettings(TradingConditionEntity tradingCondition, string? pwd = null)
        {
            if (!ValidatePassword(pwd))
                return Unauthorized();

            if (tradingCondition == null)
                return BadRequest();

            var tradingProfileKey = new TradingProfileKey(tradingCondition.TradingProfileId);
            if (!tradingProfileKey.IsValid())
                return BadRequest();

            var tradingProfile = await _tradingProfileRepositoryService.GetByKey(tradingProfileKey);
            if (tradingProfile == null)
            {
                tradingProfile = new TradingProfileEntity(tradingProfileKey);
                await _tradingProfileRepositoryService.CreateOrUpdate(tradingProfile);
            }

            var result = await _tradingConditionRepositoryService.CreateOrUpdate(tradingCondition);

            return result != null ? Ok() : BadRequest();
        }
    }
}