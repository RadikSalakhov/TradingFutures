using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TradingFutures.Application.Abstraction;
using TradingFutures.Application.Abstraction.RepositoryServices;
using TradingFutures.Application.Configuration;
using TradingFutures.Server.Controllers.Base;
using TradingFutures.Shared.Entities;

namespace TradingFutures.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradingController : BaseWebApiController
    {
        private readonly IHuobiClientService _huobiClientService;

        private readonly ISettingsItemRepositoryService _settingsItemRepository;
        private readonly ITradingPositionSettingsRepositoryService _tradingPositionSettingsRepository;

        public TradingController(IOptions<DevOptions> devOptions,
            IHuobiClientService huobiClientService,
            ISettingsItemRepositoryService settingsItemRepository,
            ITradingPositionSettingsRepositoryService tradingPositionSettingsRepository)
            : base(devOptions)
        {
            _huobiClientService = huobiClientService;
            _settingsItemRepository = settingsItemRepository;
            _tradingPositionSettingsRepository = tradingPositionSettingsRepository;
        }

        [HttpPost("update-server-settings")]
        public async Task<ActionResult> UpdateServerSettings(ServerSettingsEntity serverSettings, string? pwd = null)
        {
            if (!ValidatePassword(pwd))
                return Unauthorized();

            if (serverSettings == null)
                return BadRequest();

            var result = await _settingsItemRepository.SaveServerSettings(serverSettings);

            return result != null ? Ok() : BadRequest();
        }

        [HttpPost("update-trading-position-settings")]
        public async Task<ActionResult> UpdateTradingPositionSettings(TradingPositionSettingsEntity tradingPositionSettings, string? pwd = null)
        {
            if (!ValidatePassword(pwd))
                return Unauthorized();

            if (!(tradingPositionSettings?.Key?.IsValid() ?? false))
                return BadRequest();

            var result = await _tradingPositionSettingsRepository.CreateOrUpdate(tradingPositionSettings);

            return result != null ? Ok() : BadRequest();
        }

        [HttpGet("open-long")]
        public async Task<ActionResult> OpenLong(string asset, string? pwd = null)
        {
            if (!ValidatePassword(pwd))
                return Unauthorized();

            var result = await _huobiClientService.OpenLong(asset, "MANUAL");

            return result ? Ok(result) : BadRequest();
        }

        [HttpGet("close-long")]
        public async Task<ActionResult> CloseLong(string asset, string? pwd = null)
        {
            if (!ValidatePassword(pwd))
                return Unauthorized();

            var result = await _huobiClientService.CloseLong(asset, "MANUAL");

            return result ? Ok(result) : BadRequest();
        }

        [HttpGet("open-short")]
        public async Task<ActionResult> OpenShort(string asset, string? pwd = null)
        {
            if (!ValidatePassword(pwd))
                return Unauthorized();

            var result = await _huobiClientService.OpenShort(asset, "MANUAL");

            return result ? Ok(result) : BadRequest();
        }

        [HttpGet("close-short")]
        public async Task<ActionResult> CloseShort(string asset, string? pwd = null)
        {
            if (!ValidatePassword(pwd))
                return Unauthorized();

            var result = await _huobiClientService.CloseShort(asset, "MANUAL");

            return result ? Ok(result) : BadRequest();
        }
    }
}